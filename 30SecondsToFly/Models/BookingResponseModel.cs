using Newtonsoft.Json;

namespace Booking.Models
{
    public class BookingResponseModel
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }
        [JsonProperty("destination")]
        public string Destination { get; set; }
        [JsonProperty("outboundDeparture")]
        public DateTime OutboundDeparture { get; set; }
        [JsonProperty("outboundArrival")]
        public DateTime OutboundArrival { get; set; }
        [JsonProperty("outboundDuration")]
        public int OutboundDuration { get; set; }
        [JsonProperty("outboundAirline")]
        public string OutboundAirline { get; set; }
        [JsonProperty("outboundFlightNo")]
        public string OutboundFlightNo { get; set; }
        [JsonProperty("returnDeparture")]
        public DateTime? ReturnDeparture { get; set; }
        [JsonProperty("returnArrival")]
        public DateTime? ReturnArrival { get; set; }
        [JsonProperty("returnDuration")]
        public int? ReturnDuration { get; set; }
        [JsonProperty("returnAirline")]
        public string? ReturnAirline { get; set; }
        [JsonProperty("returnFlightNo")]
        public string? ReturnFlightNo { get; set; }
        [JsonProperty("passengers")]
        public List<PassengerModel> Passengers { get; set; }
        [JsonProperty("bookingReferenceNo")]
        public string BookingReferenceNo { get; set; }

    }
}
