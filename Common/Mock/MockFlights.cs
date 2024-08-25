using Common.Contexts.Models;
using Newtonsoft.Json;

namespace Common.Mock
{
    public class MockFlights
    {
        private List<FlightTableModel> flights = new List<FlightTableModel>();

        public MockFlights()
        {
            LoadFromJson();
        }

        public List<FlightTableModel> GetFlights(string origin, string destination, DateTime departureDate)
        {
            return flights.FindAll(p => p.Origin == origin && p.Destination == destination && p.DepartureTime.Date == departureDate.Date).ToList();
        }

        public FlightTableModel GetFlightById(int id)
        {
            return flights.Find(p => p.Id == id);
        }

        private void LoadFromJson()
        {
            var path = "flights.json";
            using (StreamReader r = new StreamReader(path))
            {
                var json = r.ReadToEnd();
                flights = JsonConvert.DeserializeObject<List<FlightTableModel>>(json);
            }
        }
    }
}
