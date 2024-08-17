﻿using Common.Exceptions.Responses;
using System.Net;

namespace Common.Exceptions
{
    public class NoFlightsFoundException : Exception
    {
        public NoFlightsFoundException() : base(ExceptionResponse.NoFlightsFoundException) { }

        public NoFlightsFoundException(string message) : base(message) { }

        public string GetErrorCode()
        {
            return ExceptionCodes.NoFlightsFoundCode;
        }

        public HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.NoContent;
        }

        public object GetObjectData()
        {
            return null;
        }
    }
}
