using Booking.Models;
using Booking.Services.Interfaces;
using Common.Contexts.Models;
using Common.Exceptions;
using Common.Logs;
using Common.Repositories;
using Common.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Common.Constants;
using Common.Utilities;
using System.Collections.Specialized;

namespace Booking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IRepository<BookingTableModel> bookingRepo;
        private readonly string serviceName = "Booking Service";
        private readonly NameValueCollection PaymentGatewayURI = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("ForwardURIs");
        public BookingService(IRepository<BookingTableModel> bookingRepo) 
        {
            this.bookingRepo = bookingRepo;
        }

        public BookingResponseModel BookFlight(BookingRequestModel model)
        {
            GlobalLoggingHandler.Logging.Info($"{serviceName}[BookFlight] Start method");
            if (model == null)
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[BookFlight] Model is null");
                throw new InvalidParameterException("Model is null");
            }

            if (model.OutboundFlightId == 0 || model.TotalPrice == 0 || model.Passengers.Count() == 0 || String.IsNullOrEmpty(model.PaymentMethod))
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[BookFlight] Invalid parameter | {model}");
                throw new InvalidParameterException();
            }

            List<PassengerModel> duplicateBooking = new List<PassengerModel>();

            foreach(var passenger in model.Passengers)
            {
                var booking = bookingRepo.Find(p => (p.FlightFK == model.OutboundFlightId || p.FlightFK == model.ReturnFlightId) && p.Name == passenger.Name && 
                    p.Surname == passenger.Surname && p.PassportCountry == passenger.PassportCountry && p.PassportNo == passenger.PassportNo);
                if(booking != null)
                {
                    duplicateBooking.Add(passenger);
                }
            }

            if(duplicateBooking.Count() > 0)
            {
                GlobalLoggingHandler.Logging.Warn($"{serviceName}[BookFlight] Duplicate bookings found for passengers | {duplicateBooking}");
                var ex = new DuplicateBookingException();
                ex.SetObject(duplicateBooking);
                throw ex;
            }

            string payment = Decryptor.DecryptText(model.PaymentMethod);

            PaymentModel paymentModel = new PaymentModel();

            try
            {
                paymentModel = JsonConvert.DeserializeObject<PaymentModel>(payment);
            }
            catch
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[BookFlight] Payment Method is invalid");
                throw new InvalidParameterException("Payment Method is invalid");
            }

            string bookingRef = Util.GenerateReference(8);

            PaymentResponseModel paymentResult = new PaymentResponseModel();

            if (paymentModel.PaymentType == (int)PaymentType.CreditCard)
            {
                CreditCardPaymentModel ccBookingInfo = new CreditCardPaymentModel()
                {
                    Name = paymentModel.Name,
                    CardNo = paymentModel.CardNo,
                    CID = paymentModel.CID,
                    Expiration = paymentModel.Expiration,
                    Amount = model.TotalPrice,
                    BookingReferenceNo = bookingRef
                };

                var ccInfo = JsonConvert.SerializeObject(ccBookingInfo).ToString();

                var encryptedCCInfo = Encryptor.EncryptText(ccInfo);

                paymentResult = HandleCreditCardPayment(encryptedCCInfo).Result;

                if (!paymentResult.CompletedTransaction)
                {
                    GlobalLoggingHandler.Logging.Warn($"{serviceName}[BookFlight] Credit card payment was unsuccessful");
                    throw new PaymentUnsuccessfulException();
                }
            } 
            else
            {
                //For other payment types
                throw new NotImplementedException();
            }

            List<BookingTableModel> bookings = new List<BookingTableModel>();
            foreach (var passenger in model.Passengers)
            {
                bookings.Add(ConvertToBookingTableModel(passenger, bookingRef, model.OutboundFlightId, model.FareClass));
                if(model.ReturnFlightId.HasValue)
                {
                    bookings.Add(ConvertToBookingTableModel(passenger, bookingRef, model.ReturnFlightId.Value, model.FareClass));
                }
            }

            GlobalLoggingHandler.Logging.Info($"{serviceName}[BookFlight] Insert bookings into database | Booking Ref: {bookingRef}");
            bookingRepo.AddRange(bookings);
            bookingRepo.SaveChanges();

            var response = new BookingResponseModel()
            {
                BookingReferenceNo = bookingRef
            };

            GlobalLoggingHandler.Logging.Info($"{serviceName}[BookFlight] End method");
            return response;
        }

        private async Task<PaymentResponseModel> HandleCreditCardPayment(string paymentMethod)
        {
            var ccURI = PaymentGatewayURI?["CCPaymentGateway"].ToString();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ccURI);
            requestMessage.Content = JsonContent.Create(paymentMethod);

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var responseMsg = await client.SendAsync(requestMessage).ConfigureAwait(false);
                var contentString = await responseMsg.Content.ReadAsStringAsync().ConfigureAwait(false);

                try
                {
                    PaymentResponseModel responseItem = JsonConvert.DeserializeObject<PaymentResponseModel>(contentString);
                    return responseItem;
                }
                catch
                {
                    GlobalLoggingHandler.Logging.Error($"{serviceName}[HandleCreditCardPayment] Failed to convert Json object | {contentString}");
                    throw new GeneralException();
                }
            }
        }

        private BookingTableModel ConvertToBookingTableModel(PassengerModel model, string bookingRef, int flightFK, int fareClass)
        {
            BookingTableModel booking = new BookingTableModel();

            booking.BookingReference = bookingRef;
            booking.FlightFK = flightFK;
            booking.Name = model.Name;
            booking.Surname = model.Surname;
            booking.PassportCountry = model.PassportCountry;
            booking.PassportNo = model.PassportNo;
            booking.DateOfBirth = model.DateOfBirth;
            booking.FareClass = fareClass;

            return booking;
        }
    }
}
