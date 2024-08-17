using NLog.Targets;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NLog.LayoutRenderers;
using NLog;
using NLog.Config;
using System.Configuration;

namespace Common.Logs
{
    public class GlobalLoggingHandler
    {
        private static GlobalLoggingHandler logger;
        private static bool isServiceRestart = false;
        private static NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("TSFLog");
        private static string formatDate = "yyyy-MM-dd";
        private static string formatDateTime = "yyyy-MM-dd_HHmm";

        private GlobalLoggingHandler()
        {
        }

        public static GlobalLoggingHandler Logging
        {
            get
            {
                if (logger == null)
                {
                    logger = new GlobalLoggingHandler();
                    isServiceRestart = true;
                }
                else
                    isServiceRestart = false;

                return logger;
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void Debug(string message,
            [CallerMemberName] string callerMethodName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            LogMessage(LogLevel.Debug,
                message,
                callerMethodName,
                callerFilePath,
                callerLineNumber,
                Path.GetFileNameWithoutExtension(new StackTrace().GetFrame(1).GetMethod().Module.Name));
        }

        public void Info(string message,
            [CallerMemberName] string callerMethodName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            LogMessage(LogLevel.Info,
                message,
                callerMethodName,
                callerFilePath,
                callerLineNumber,
                Path.GetFileNameWithoutExtension(new StackTrace().GetFrame(1).GetMethod().Module.Name));
        }

        public void Warn(string message,
            [CallerMemberName] string callerMethodName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            LogMessage(LogLevel.Warn,
               message,
               callerMethodName,
               callerFilePath,
               callerLineNumber,
               Path.GetFileNameWithoutExtension(new StackTrace().GetFrame(1).GetMethod().Module.Name));
        }

        public void Error(string message,
            [CallerMemberName] string callerMethodName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            LogMessage(LogLevel.Error,
               message,
               callerMethodName,
               callerFilePath,
               callerLineNumber,
               Path.GetFileNameWithoutExtension(new StackTrace().GetFrame(1).GetMethod().Module.Name));
        }

        public void Fatal(string message, Exception ex)
        {
            StackFrame stackFrame = new StackTrace(ex, true).GetFrame(0); // 0 = Display the most recent function call.
            LogMessage(LogLevel.Fatal,
                message + ":" + ex,
                stackFrame.GetMethod().Name,
                stackFrame.GetFileName(),
                stackFrame.GetFileLineNumber(),
                Path.GetFileNameWithoutExtension(stackFrame.GetMethod().Module.Name));
        }

        private void LogMessage(LogLevel logLevel, string message, string callerMethodName, string callerFilePath, int callerLineNumber, string callerProjectName)
        {
            RegisterLayoutRenderer(callerMethodName, callerFilePath, callerLineNumber, callerProjectName);
            NLogConfiguration(callerProjectName);
            Logger logger = LogManager.GetLogger(callerProjectName);
            logger.Log(logLevel, message);
        }

        private void NLogConfiguration(string callerProjectName)
        {
            string directory = section?["Path"] ?? "C:/30SecondsToFly/logs";

            if (isServiceRestart)
            {
                ArchiveLogFile(directory, callerProjectName);
                MoveLogFile(directory, callerProjectName);
            }
            else
                MoveLogFile(directory, callerProjectName);

            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget();
            config.AddTarget("logfile", fileTarget);

            fileTarget.FileName = directory + "/" + (section?["Filename"] ?? GetFileName(callerProjectName, formatDate));
            fileTarget.Layout = section?["Layout"] ?? $"${{longdate}} | ${{level}} | ${{callerProjectName}} | ${{message}} | ${{callerMethodName}} | ${{callerFilePath}} | ${{callerLineNumber}}";

            string archivePath = section?["ArchivePath"] ?? GetArchiveDirectory(directory);
            string archiveFileName = section?["ArchiveFileName"] ?? $"{{#}}_{callerProjectName}.log";
            fileTarget.ArchiveFileName = $"{archivePath}/{archiveFileName}";

            fileTarget.ArchiveEvery = (FileArchivePeriod)Enum.Parse(typeof(FileArchivePeriod), section?["ArchiveEvery"] ?? "Day");
            fileTarget.ArchiveNumbering = (ArchiveNumberingMode)Enum.Parse(typeof(ArchiveNumberingMode), section?["ArchiveNumbering"] ?? "Date");
            fileTarget.ArchiveDateFormat = section?["ArchiveDateFormat"] ?? formatDateTime;
            fileTarget.ConcurrentWrites = section?["ConcurrentWrites"] != null ? Convert.ToBoolean(section["ConcurrentWrites"]) : true;
            fileTarget.KeepFileOpen = section?["KeepFileOpen"] != null ? Convert.ToBoolean(section["KeepFileOpen"]) : false;
            fileTarget.ArchiveAboveSize = section?["ArchiveAboveSize"] != null ? Convert.ToInt32(section["ArchiveAboveSize"]) : 2048000;
            fileTarget.Encoding = Encoding.UTF8;

            var rule = new LoggingRule("*", NLog.LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }

        private void ArchiveLogFile(string directory, string callerProjectName)
        {
            string folderArchive = GetArchiveDirectory(directory);
            if (!Directory.Exists(folderArchive))
                Directory.CreateDirectory(folderArchive);

            string filePath = GetDirectory(directory, callerProjectName);
            string newFilePath = $"{folderArchive}/{GetFileName(callerProjectName, formatDateTime)}";
            if (File.Exists(filePath))
            {
                int count = 1;
                string fileNameOnly = Path.GetFileNameWithoutExtension(newFilePath);
                string extension = Path.GetExtension(newFilePath);
                string path = Path.GetDirectoryName(newFilePath);
                string newFullPath = newFilePath;

                while (File.Exists(newFullPath))
                {
                    string tempFileName = $"{fileNameOnly}({count++})";
                    newFullPath = Path.Combine(path, tempFileName + extension);
                }

                File.Move(filePath, newFullPath);
            }

            DeleteLogFile(folderArchive);
        }

        private void MoveLogFile(string directory, string callerProjectName)
        {
            string[] files = Directory.GetFiles(section?["Path"] ?? "C:/30SecondsToFly/logs");
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastWriteTime < DateTime.Now.AddDays(-1))
                {
                    string folderArchive = GetArchiveDirectory(directory);
                    if (!Directory.Exists(folderArchive))
                        Directory.CreateDirectory(folderArchive);

                    string filePath = fi.FullName;
                    string newFilePath = $"{folderArchive}/{fi.Name}";
                    if (File.Exists(filePath))
                    {
                        int count = 1;
                        string fileNameOnly = Path.GetFileNameWithoutExtension(newFilePath);
                        string extension = Path.GetExtension(newFilePath);
                        string path = Path.GetDirectoryName(newFilePath);
                        string newFullPath = newFilePath;

                        while (File.Exists(newFullPath))
                        {
                            string tempFileName = $"{fileNameOnly}({count++})";
                            newFullPath = Path.Combine(path, tempFileName + extension);
                        }

                        File.Move(filePath, newFullPath);
                    }
                }
            }
        }

        private void DeleteLogFile(string folderArchive)
        {
            int deleteDate = section?["DeleteDate"] != null ? Convert.ToInt32(section?["DeleteDate"]) : 0;
            if (deleteDate > 0)
            {
                deleteDate = -deleteDate;
                string[] files = Directory.GetFiles(folderArchive);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddDays(deleteDate))
                        fi.Delete();
                }

                files = Directory.GetFiles(section?["Path"] ?? "C:/30SecondsToFly/logs");
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddDays(deleteDate))
                        fi.Delete();
                }
            }
        }

        private void RegisterLayoutRenderer(string callerMethodName, string callerFilePath, int callerLineNumber, string callerProjectName)
        {
            LayoutRenderer.Register("callerProjectName", (logEvent) => callerProjectName);
            LayoutRenderer.Register("callerMethodName", (logEvent) => callerMethodName);
            LayoutRenderer.Register("callerLineNumber", (logEvent) => callerLineNumber);
            LayoutRenderer.Register("callerFilePath", (logEvent) => callerFilePath);
        }

        private string GetDirectory(string directory, string callerProjectName)
        {
            return $"{directory}/{GetFileName(callerProjectName, formatDate)}";
        }

        private string GetArchiveDirectory(string directory)
        {
            return $"{directory}/archives";
        }

        private string GetFileName(string filename, string formatDateTime)
        {
            return $"{DateTime.Now.ToString(formatDateTime)}_{filename}.log";
        }
    }
}
