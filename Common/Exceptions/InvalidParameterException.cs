using Common.Exceptions.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class InvalidParameterException : Exception
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

        public object GetObjectData()
        {
            return null;
        }
    }
}
