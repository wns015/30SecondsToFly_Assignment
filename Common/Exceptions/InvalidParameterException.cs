
using Common.Exceptions.Responses;
using System.Net;
namespace Common.Exceptions
{
    public class InvalidParameterException : BaseException
    {
        public InvalidParameterException() : base(ExceptionResponse.InvalidParamterException) { }

        public InvalidParameterException(string message) : base(message) { }

        public string GetErrorCode()
        {
            return ExceptionCodes.InvalidParamterCode;
        }

        public HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.BadRequest;
        }

        public object GetObject()
        {
            return null;
        }
    }
}
