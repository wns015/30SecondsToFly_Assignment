using Newtonsoft.Json;

namespace Booking.Models
{
    public class BookingResponseModel
    {
        [JsonProperty("bookingReferenceNo")]
        public string BookingReferenceNo { get; set; }

    }
}
