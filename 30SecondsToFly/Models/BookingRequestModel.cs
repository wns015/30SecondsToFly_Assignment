using Newtonsoft.Json;

namespace Booking.Models
{
    public class BookingRequestModel
    {
        [JsonProperty("outboundFlightId")]
        public int OutboundFlightId { get; set; }
        [JsonProperty("returnFlightId")]
        public int? ReturnFlightId { get; set; }
        [JsonProperty("totalPrice")]
        public double TotalPrice { get; set; }
        [JsonProperty("passengers")]
        public PassengerModel[] Passengers { get; set; }
        [JsonProperty("paymentMethod")]
        public int PaymentMethod { get; set; }
        [JsonProperty("paymentDetails")]
        public string PaymentDetails { get; set; }
        [JsonProperty("fareClass")]
        public int FareClass {  get; set; }

    }
}
