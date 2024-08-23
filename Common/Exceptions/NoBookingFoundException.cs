using Common.Exceptions.Responses;
using System.Net;

namespace Common.Exceptions
{
    public class NoBookingFoundException : BaseException
    {
        public NoBookingFoundException() : base(ExceptionResponse.NoFlightsFoundException) { }

        public NoBookingFoundException(string message) : base(message) { }

        public string GetErrorCode()
        {
            return ExceptionCodes.NoBookingFoundCode;
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
