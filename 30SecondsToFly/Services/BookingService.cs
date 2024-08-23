using Booking.Models;
using Booking.Services.Interfaces;
using Common.Contexts.Models;
using Common.Exceptions;
using Common.Logs;
using Common.Repositories;
using Common.Security;
using Newtonsoft.Json;
using Common.Constants;
using Common.Utilities;
using System.Collections.Specialized;
using Common.Models;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace Booking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IConfiguration config;
        private readonly IRepository<BookingTableModel> bookingRepo;
        private readonly IRepository<FlightTableModel> flightRepo;
        private readonly string serviceName = "Booking Service";
        public BookingService(IRepository<BookingTableModel> bookingRepo, IRepository<FlightTableModel> flightRepo, IConfiguration config) 
        {
            this.bookingRepo = bookingRepo;
            this.flightRepo = flightRepo;
            this.config = config;
        }

        public BookingResponseModel BookFlight(BookingRequestModel model)
        {
            GlobalLoggingHandler.Logging.Info($"{serviceName}[BookFlight] Start method");
            if (model == null)
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[BookFlight] Model is null");
                throw new InvalidParameterException("Model is null");
            }

            if (model.OutboundFlightId == 0 || model.TotalPrice == 0 || model.Passengers.Count() == 0 || model.PaymentMethod == 0)
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

            string bookingRef = Util.GenerateReference(8);

            PaymentResponseModel paymentResult = new PaymentResponseModel();

            if (model.PaymentMethod == (int)PaymentType.CreditCard)
            {
                BookingPaymentModel bookingPaymentInfo = new BookingPaymentModel()
                {
                    PaymentDetails = model.PaymentDetails,
                    Amount = model.TotalPrice,
                    BookingReferenceNo = bookingRef
                };

                paymentResult = HandleCreditCardPayment(bookingPaymentInfo).Result;

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

            var response = GetBooking(bookingRef);

            GlobalLoggingHandler.Logging.Info($"{serviceName}[BookFlight] End method");
            return response;
        }

        public BookingResponseModel SearchBooking(BookingSearchModel model)
        {
            GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchBooking] Start method");

            if (model == null || String.IsNullOrEmpty(model.Surname) || String.IsNullOrEmpty(model.BookingReferenceNo))
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[SearchBooking] Invalid model | {model}");
                throw new InvalidParameterException(); 
            }

            var booking = bookingRepo.Find(p => p.BookingReference == model.BookingReferenceNo && p.Surname == model.Surname);

            if (booking == null)
            {
                GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchBooking] No booking found | {model}");
                return null;
            }

            var response = GetBooking(model.BookingReferenceNo);

            return response;
        }

        private async Task<PaymentResponseModel> HandleCreditCardPayment(BookingPaymentModel paymentMethod)
        {
            
            var uris = config.GetSection("ForwardURIs");
            var ccURI = uris["CCPaymentGateway"];
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ccURI);
            requestMessage.Content = JsonContent.Create(paymentMethod);

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                var responseMsg = await client.SendAsync(requestMessage).ConfigureAwait(false);
                var contentString = await responseMsg.Content.ReadAsStringAsync().ConfigureAwait(false);

                try
                {
                    var responseItem = JObject.Parse(contentString);
                    var response  =  JsonConvert.DeserializeObject<PaymentResponseModel>(JsonConvert.SerializeObject(responseItem["data"]));

                    return response;
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

        private PassengerModel ConvertBookingToPassengerModel(BookingTableModel model)
        {
            PassengerModel passenger = new PassengerModel();

            passenger.Name = model.Name;
            passenger.Surname = model.Surname;
            passenger.DateOfBirth = model.DateOfBirth;
            passenger.PassportCountry = model.PassportCountry;
            passenger.PassportNo = model.PassportNo;

            return passenger;
        }

        private BookingResponseModel GetBooking(string refNo)
        {
            var result = new BookingResponseModel();
            var bookingList = bookingRepo.FindAll(p => p.BookingReference == refNo).ToList();

            var flightList = new List<FlightTableModel>();

            var flightIdList = bookingList.Select(p => p.FlightFK).Distinct().ToList();

            foreach (var flight in flightIdList)
            {
                flightList.Add(flightRepo.Find(p => p.Id == flight));
            }

            flightList = flightList.OrderBy(p => p.DepartureTime).ToList();

            if (flightList.Count == 2)
            {
                result.ReturnDeparture = flightList[1].DepartureTime;
                result.ReturnArrival = flightList[1].ArrivalTime;
                result.ReturnDuration = flightList[1].Duration;
                result.ReturnAirline = flightList[1].Airline;
                result.ReturnFlightNo = flightList[1].FlightNo;
            }

            result.Origin = flightList[0].Origin;
            result.Destination = flightList[0].Destination;
            result.OutboundDeparture = flightList[0].DepartureTime;
            result.OutboundArrival = flightList[0].ArrivalTime;
            result.OutboundDuration = flightList[0].Duration;
            result.OutboundAirline = flightList[0].Airline;
            result.OutboundFlightNo = flightList[0].FlightNo;

            var filteredPassengers = bookingList.ToList().DistinctBy(p => p.PassportNo);

            var passengers = new List<PassengerModel>();

            foreach (var passenger in filteredPassengers)
            {
                passengers.Add(ConvertBookingToPassengerModel(passenger));
            }


            result.Passengers = passengers;
            result.BookingReferenceNo = bookingList[0].BookingReference;

            GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchBooking] End method");
            return result;
        }
    }
}
