using Newtonsoft.Json;

namespace Booking.Models
{
    public class BookingPaymentModel
    {
        [JsonProperty("bookingReferenceNo")]
        public string BookingReferenceNo { get; set; }
        [JsonProperty("paymentDetails")]
        public string PaymentDetails { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
    }
}
