using Booking.Models;
using Booking.Services.Interfaces;
using Common.Contexts.Models;
using Common.Exceptions;
using Common.Logs;
using Common.Repositories;
using Newtonsoft.Json;
using Common.Constants;
using Common.Utilities;
using Newtonsoft.Json.Linq;
using Common.Security;

namespace Booking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IConfiguration config;
        private readonly IRepository<BookingTableModel> bookingRepo;
        private readonly IRepository<FlightTableModel> flightRepo;
        private readonly IRepository<PassengerDetailTableModel> passengerRepo;
        private readonly string serviceName = "Booking Service";
        public BookingService(IRepository<BookingTableModel> bookingRepo, IRepository<FlightTableModel> flightRepo, IConfiguration config, IRepository<PassengerDetailTableModel> passengerRepo)
        {
            this.bookingRepo = bookingRepo;
            this.flightRepo = flightRepo;
            this.config = config;
            this.passengerRepo = passengerRepo;
        }

        public TransmissionModel BookFlight(TransmissionModel requestModel)
        {
            try
            {
                GlobalLoggingHandler.Logging.Info($"{serviceName}[BookFlight] Start method");

                var objectString = Decryptor.DecryptText(requestModel.EncryptedString);
                BookingRequestModel model = JsonConvert.DeserializeObject<BookingRequestModel>(objectString);
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


                var returnFlightId = model.ReturnFlightId.HasValue ? model.ReturnFlightId.Value : 0;


                List<PassengerDetailTableModel> duplicatePassenger = new List<PassengerDetailTableModel>();

                foreach (var passenger in model.Passengers)
                {
                    var passengerCheck = passengerRepo.FindAll(p => p.Name == passenger.Name && p.Surname == passenger.Surname
                        && p.PassportNo == passenger.PassportNo && p.PassportIssuer == passenger.PassportIssuer && p.DateOfBirth == passenger.DateOfBirth).ToList();

                    if (passengerCheck != null)
                    {
                        duplicatePassenger.Concat(passengerCheck).ToList();
                    }
                };

                List<BookingTableModel> duplicateBooking = new List<BookingTableModel>();

                if (duplicatePassenger.Count > 0)
                {
                    var bookingList = duplicatePassenger.Select(p => p.BookingReferenceNo).Distinct().ToList();

                    foreach (var booking in bookingList)
                    {
                        var bookingCheck = bookingRepo.Find(p => p.BookingReferenceNo == booking);
                        if (bookingCheck != null)
                        {
                            if (bookingCheck.OutboundFlightFK == model.OutboundFlightId || bookingCheck.ReturnFlightFK == returnFlightId)
                            {
                                duplicateBooking.Add(bookingCheck);
                            }
                        }
                    }
                }


                if (duplicateBooking.Count() > 0)
                {
                    GlobalLoggingHandler.Logging.Warn($"{serviceName}[BookFlight] Duplicate bookings found for passengers | {model.Passengers}");
                    throw new DuplicateBookingException();

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



                GlobalLoggingHandler.Logging.Info($"{serviceName}[BookFlight] Save booking information to database | Booking Ref: {bookingRef}");
                try
                {
                    SaveBooking(model, bookingRef);
                }
                catch
                {
                    GlobalLoggingHandler.Logging.Error($"{serviceName}[BookFlight] Failed to insert information into database {model}");
                    throw new GeneralException();
                }

                var result = GetBooking(bookingRef);

                TransmissionModel response = new TransmissionModel()
                {
                    EncryptedString = Encryptor.EncryptText(JsonConvert.SerializeObject(result))
                };

                GlobalLoggingHandler.Logging.Info($"{serviceName}[BookFlight] End method");
                return response;
            }
            catch
            {
                throw new GeneralException();
            }
        }

        public TransmissionModel SearchBooking(TransmissionModel requestModel)
        {
            try
            {
                GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchBooking] Start method");
                var objectString = Decryptor.DecryptText(requestModel.EncryptedString);
                BookingSearchModel model = JsonConvert.DeserializeObject<BookingSearchModel>(objectString);

                if (model == null || String.IsNullOrEmpty(model.Surname) || String.IsNullOrEmpty(model.BookingReferenceNo))
                {
                    GlobalLoggingHandler.Logging.Error($"{serviceName}[SearchBooking] Invalid model | {model}");
                    throw new InvalidParameterException();
                }

                var booking = passengerRepo.Find(p => p.BookingReferenceNo == model.BookingReferenceNo && p.Surname == model.Surname);

                if (booking == null)
                {
                    GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchBooking] No booking found | {model}");
                    return null;
                }

                var result = GetBooking(model.BookingReferenceNo);

                TransmissionModel response = new TransmissionModel()
                {
                    EncryptedString = Encryptor.EncryptText(JsonConvert.SerializeObject(result))
                };

                return response;
            }
            catch (Exception ex) 
            {
                if (ex is InvalidParameterException)
                {
                    throw ex;
                }
                GlobalLoggingHandler.Logging.Error($"{serviceName}[SearchBooking] Error occured {ex.InnerException}");
                throw new GeneralException();
            }
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

        private void SaveBooking(BookingRequestModel model, string bookingRef)
        {
            BookingTableModel booking = new BookingTableModel();

            booking.BookingReferenceNo = bookingRef;
            booking.OutboundFlightFK = model.OutboundFlightId;
            booking.ReturnFlightFK = model.ReturnFlightId.HasValue ? model.ReturnFlightId.Value : null;
            booking.FareClass = model.FareClass;
            booking.Email = model.Email;

            List<PassengerDetailTableModel> passengers = new List<PassengerDetailTableModel>();

            foreach(var passenger in model.Passengers)
            {
                var insert = new PassengerDetailTableModel();
                insert.Name = passenger.Name;
                insert.Surname = passenger.Surname;
                insert.PassportIssuer = passenger.PassportIssuer;
                insert.PassportNo = passenger.PassportNo;
                insert.DateOfBirth = passenger.DateOfBirth;
                insert.BookingReferenceNo = bookingRef;
                
                passengers.Add(insert);
            }

            bookingRepo.Add(booking);
            passengerRepo.AddRange(passengers);
            bookingRepo.SaveChanges();
            passengerRepo.SaveChanges();
        }

        private PassengerModel PassengerDetailsToPassengerModel(PassengerDetailTableModel model)
        {
            PassengerModel passenger = new PassengerModel();

            passenger.Name = model.Name;
            passenger.Surname = model.Surname;
            passenger.DateOfBirth = model.DateOfBirth;
            passenger.PassportIssuer = model.PassportIssuer;
            passenger.PassportNo = model.PassportNo;

            return passenger;
        }

        private BookingResponseModel GetBooking(string refNo)
        {
            var result = new BookingResponseModel();
            var passengerList = passengerRepo.FindAll(p => p.BookingReferenceNo == refNo).ToList();

            var booking = bookingRepo.Find(p => p.BookingReferenceNo == refNo);

            var outboundFlight = flightRepo.Find(p => p.Id == booking.OutboundFlightFK);

            result.Origin = outboundFlight.Origin;
            result.Destination = outboundFlight.Destination;
            result.OutboundDeparture = outboundFlight.DepartureTime;
            result.OutboundArrival = outboundFlight.ArrivalTime;
            result.OutboundDuration = outboundFlight.Duration;
            result.OutboundAirline = outboundFlight.Airline;
            result.OutboundFlightNo = outboundFlight.FlightNo;

            if (booking.ReturnFlightFK.HasValue)
            {
                var returnFlight = flightRepo.Find(p => p.Id == booking.ReturnFlightFK);
                result.ReturnFlightNo = returnFlight.FlightNo;
                result.ReturnDuration = returnFlight.Duration;
                result.ReturnDeparture = returnFlight.DepartureTime;
                result.ReturnArrival = returnFlight.ArrivalTime;
                result.ReturnAirline = returnFlight.Airline;
            }

            var passengers = new List<PassengerModel>();

            foreach (var passenger in passengerList)
            {
                passengers.Add(PassengerDetailsToPassengerModel(passenger));
            }


            result.Passengers = passengers;
            result.BookingReferenceNo = refNo;

            GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchBooking] End method");
            return result;
        }
    }
}
