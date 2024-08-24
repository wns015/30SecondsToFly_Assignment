using Booking.Models;
using Booking.Services.Interfaces;
using Common.Constants;
using Common.Contexts.Models;
using Common.Exceptions;
using Common.Logs;
using Common.Repositories;
using System.Reflection;

namespace Booking.Services
{
    public class SearchService : ISearchService
    {
        public readonly IRepository<FlightTableModel> flightRepo;
        public readonly IRepository<BookingTableModel> bookingRepo;
        public readonly IRepository<PassengerDetailTableModel> passengerRepo;
        public readonly string serviceName = "[Search Service]";
        public FareClass fareClass = new FareClass();

        public SearchService(IRepository<FlightTableModel> flightRepo, IRepository<BookingTableModel> bookingRepo, IRepository<PassengerDetailTableModel> passengerRepo)
        {
            this.flightRepo = flightRepo;
            this.bookingRepo = bookingRepo;
            this.passengerRepo = passengerRepo;
        }

        public SearchResultModel SearchFlights(SearchModel model)
        {
            GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchFlights] Start method");
            bool invalidParameter = false;

            foreach (PropertyInfo prop in model.GetType().GetProperties())
            {
                if (prop.Name != "ReturnDate")
                {
                    if (prop.GetValue(model) == null)
                        invalidParameter = true;
                }
            }

            if (invalidParameter)
            {
                GlobalLoggingHandler.Logging.Error($"{serviceName}[SearchFlights] One or more values are null | {model}");
                throw new InvalidParameterException();
            }

            var result = new SearchResultModel();

            if (!model.IsOneWay)
            {
                var returnDate = DateTime.Parse(model.ReturnDate);
                List<FlightModel> returnFlights = GetFlights(model.Destination, model.Origin, returnDate, model.FareClass, model.NoOfPassengers);
                if(returnFlights.Count == 0)
                {
                    GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchFlights] No return flights found | {model}");
                    return null;
                } 
                else
                {
                    result.ReturnFlights = returnFlights;
                }
            }

            var departureDate = DateTime.Parse(model.DepartureDate);
            List <FlightModel> outboundFlights = GetFlights(model.Origin, model.Destination, departureDate, model.FareClass, model.NoOfPassengers);

            if (outboundFlights.Count == 0)
            {
                return null;
            }
            else
            {
                result.OutboundFlights = outboundFlights;
            }

            GlobalLoggingHandler.Logging.Info($"{serviceName}[SearchFlights] End method");
            return result;
        }

        private List<FlightModel> GetFlights(string origin, string destination, DateTime departureDate, int fare, int passengers)
        {
            List<FlightTableModel> flights = flightRepo.FindAll(p => p.Origin == origin && p.Destination == destination && p.DepartureTime.Date == departureDate.Date).ToList();

            var filteredFare = new List<FlightTableModel>();
            foreach (var flight in flights)
            {
                foreach (PropertyInfo prop in flight.GetType().GetProperties())
                {
                    if (prop.Name == fareClass.ToFareClassPrice(fare))
                    {
                        if (prop.GetValue(flight) != null)
                        {
                            filteredFare.Add(flight);
                            break;
                        }
                    }
                }
            }

            var filteredFlights = new List<FlightTableModel>();
            foreach (var flight in filteredFare)
            {
                var bookingCheck = bookingRepo.FindAll(p => p.ReturnFlightFK == flight.Id || p.OutboundFlightFK == flight.Id).ToList();

                int bookingCount = 0;

                foreach (var booking in bookingCheck)
                {
                    bookingCount += passengerRepo.FindAll(p => p.BookingReferenceNo == booking.BookingReferenceNo).Count();
                }

                foreach (PropertyInfo prop in flight.GetType().GetProperties())
                {
                    
                    if (prop.Name == fareClass.ToFareClassSeating(fare))
                    {
                        int availableSeats = (int)prop.GetValue(flight) - bookingCount;
                        if (availableSeats >= passengers)
                        {
                            filteredFlights.Add(flight);
                            break;
                        }
                    }
                }
            }

            List<FlightModel> foundFlights = new List<FlightModel>();

            foreach (var flight in filteredFlights)
            {
                double price = 0;
                foreach (PropertyInfo prop in flight.GetType().GetProperties())
                {
                    if(prop.Name == fareClass.ToFareClassPrice(fare))
                    {
                        price = (double)prop.GetValue(flight);
                        break;
                    }
                }
                foundFlights.Add(ConvertToFlightModel(flight, price));
            }

            return foundFlights;
        }

        private FlightModel ConvertToFlightModel(FlightTableModel model, double price)
        {
            FlightModel flight = new FlightModel()
            {
                Id = model.Id,
                Origin = model.Origin,
                Destination = model.Destination,
                Airline = model.Airline,
                FlightNo = model.FlightNo,
                Duration = model.Duration,
                DepartureTime = model.DepartureTime,
                ArrivalTime = model.ArrivalTime,
                Price = price
            };

            return flight;
        }
    }
}
