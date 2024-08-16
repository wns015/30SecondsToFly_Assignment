using Newtonsoft.Json;

namespace Booking.Models
{
    public class BookingRequestModel
    {
        [JsonProperty("outboundFlightId")]
        public int OutboundFlightId { get; set; }
        [JsonProperty("outboundArrivalTime")]
        public int ReturnFlightId { get; set; }
        [JsonProperty("totalPrice")]
        public float TotalPrice { get; set; }
        [JsonProperty("passengers")]
        public PassengerModel[] Passengers { get; set; }
        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }

    }
}
