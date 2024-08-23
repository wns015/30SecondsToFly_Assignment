namespace Common.Exceptions
{
    [Serializable]
    public class BaseException : Exception
    {

        public BaseException(string message): base(message) { }

        public override string StackTrace
        {
            get
            {
                return "";
            }
        }
    }
}
