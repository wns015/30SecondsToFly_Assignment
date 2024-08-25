using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions.Responses
{
    public static class ExceptionResponse
    {
        public const string SystemError = "System Error";
        public const string DuplicateBookingError = "Duplicate booking";
        public const string NoFlightsFoundException = "No flights found";
        public const string NoBookingFound = "No booking found";
        public const string InvalidParamterException = "Invalid parameter";
        public const string PaymentUnsuccessfulException = "Payment was unsuccessful";
        public const string DuplicateUserException = "Username has been taken";
        public const string DuplicateEmailException = "Email already associated with an account";
    }
}
