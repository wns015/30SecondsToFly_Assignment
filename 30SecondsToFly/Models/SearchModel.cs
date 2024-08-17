using Newtonsoft.Json;

namespace Booking.Models
{
    public class SearchModel
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }
        [JsonProperty("destination")]
        public string Destination { get; set; }
        [JsonProperty("departureDate")]
        public DateTime DepartureDate { get; set; }
        [JsonProperty("returnDate")]
        public DateTime? ReturnDate { get; set; }
        [JsonProperty("noOfPassengers")]
        public int NoOfPassengers { get; set; }
        [JsonProperty("fareClass")]
        public int FareClass { get; set; }

    }
}
