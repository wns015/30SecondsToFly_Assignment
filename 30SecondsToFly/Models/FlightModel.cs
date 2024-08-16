using Newtonsoft.Json;

namespace Booking.Models
{
    public class FlightModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("origin")]
        public string Origin { get; set; }
        [JsonProperty("destination")]
        public string Destination { get; set; }
        [JsonProperty("departureTime")]
        public DateTime DepartureTime { get; set; }
        [JsonProperty("arrivalTime")]
        public DateTime ArrivalTime { get; set; }
        [JsonProperty("price")]
        public double Price { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("flightNo")]
        public string FlightNo { get; set; }
    }
}
