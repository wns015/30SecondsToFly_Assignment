using Newtonsoft.Json;

namespace PaymentGateway.Models
{
    public class BookingPaymentModel
    {
        [JsonProperty("bookingReferenceNo")]
        public string BookingReferenceNo { get; set; }
        [JsonProperty("paymentDetails")]
        public string PaymentDetails { get; set; }
        [JsonProperty("amount")]
        public float Amount { get; set; }
    }
}
