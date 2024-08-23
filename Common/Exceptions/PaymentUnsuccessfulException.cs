using Common.Exceptions.Responses;
using System.Net;

namespace Common.Exceptions
{
    public class PaymentUnsuccessfulException :BaseException
    {
        public PaymentUnsuccessfulException() : base(ExceptionResponse.PaymentUnsuccessfulException) { }

        public PaymentUnsuccessfulException(string message) : base(message) { }

        public string GetErrorCode()
        {
            return ExceptionCodes.PaymentUnsuccessfulCode;
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
