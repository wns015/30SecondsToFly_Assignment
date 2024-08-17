using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions.Responses
{
    public static class ExceptionCodes
    {
        public const string NoBookingFoundCode = "10001";
        public const string DuplicateBookingCode = "10002";
        public const string NoFlightsFoundCode = "10003";
        public const string PaymentUnsuccessfulCode = "10004";
        public const string InvalidParamterCode = "20001";
        public const string SystemErrorCode = "99999";
    }
}
