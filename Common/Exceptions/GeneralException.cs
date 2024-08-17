using System.Net;
using Common.Exceptions.Responses;

namespace Common.Exceptions
{
    public class GeneralException : Exception
    {
        public GeneralException() : base (ExceptionResponse.SystemError) { }

        public GeneralException(string message) : base(message) { }

        public string GetErrorCode()
        {
            return ExceptionCodes.SystemErrorCode;
        }

        public HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.InternalServerError;
        }

        public object GetObjectData()
        {
            return null;
        }
    }
}
