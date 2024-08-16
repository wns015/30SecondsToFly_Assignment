using Newtonsoft.Json;

namespace Booking.Models
{
    public class PaymentModel
    {
        [JsonProperty("paymentType")]
        public int PaymentType { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("cardNo")]
        public string CardNo { get; set; }
        [JsonProperty("cID")]
        public string CID { get; set; }
        [JsonProperty("expiration")]
        public string Expiration { get; set; }
    }
}
