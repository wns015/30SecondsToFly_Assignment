
using Common.Exceptions.Responses;
using System.Net;

namespace Common.Exceptions
{
    public class DuplicateBookingException : BaseException
    {
        private object Obj;
        public DuplicateBookingException() : base(ExceptionResponse.DuplicateBookingError) { }

        public DuplicateBookingException(string message) : base(message) { }

        public string GetErrorCode()
        {
            return ExceptionCodes.DuplicateBookingCode;
        }

        public HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.BadRequest;
        }

        public object GetObject()
        {
            if (Obj == null)
            {
                return null;
            }
            else
            {
                return Obj;
            }
        }

        public void SetObject(object obj)
        {
            this.Obj = obj;
        }
    }
}
