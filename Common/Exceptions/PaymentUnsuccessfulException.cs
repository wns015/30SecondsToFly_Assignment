using Common.Exceptions.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class PaymentUnsuccessfulException : Exception
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

        public object GetObjectData()
        {
            return null;
        }
    }
}
