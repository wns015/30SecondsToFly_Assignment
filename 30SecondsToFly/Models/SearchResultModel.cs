using Newtonsoft.Json;

namespace Booking.Models
{
    public class SearchResultModel
    {
        [JsonProperty("outboundFlights")]
        public List<FlightModel> OutboundFlights { get; set; }
        [JsonProperty("returnFlights")]
        public List<FlightModel>? ReturnFlights { get; set; }

    }
}
