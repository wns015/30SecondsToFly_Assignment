﻿using Newtonsoft.Json;

namespace Booking.Models
{
    public class PaymentResponseModel
    {
        [JsonProperty("completedTransaction")]
        public bool CompletedTransaction { get; set; }
    }
}
