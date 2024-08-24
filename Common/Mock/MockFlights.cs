using Common.Contexts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
