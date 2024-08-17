using Newtonsoft.Json;

namespace PaymentGateway.Models
{
    public class CreditCardPaymentModel
    {
        [JsonProperty("transactionReferenceNo")]
        public string TransactionReferenceNo { get; set; }
        [JsonProperty("bookingReferenceNo")]
        public string BookingReferenceNo { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("cardNo")]
        public string CardNo { get; set; }
        [JsonProperty("cID")]
        public string CID { get; set; }
        [JsonProperty("expiration")]
        public string Expiration { get; set; }
        [JsonProperty("amount")]
        public double Amount {  get; set; }
    }
}
