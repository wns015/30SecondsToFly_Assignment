using Newtonsoft.Json;

namespace PaymentGateway.Models
{
    public class PaymentResponseModel
    {
        [JsonProperty("completedTransaction")]
        public bool CompletedTransaction { get; set; }
    }
}
