using Newtonsoft.Json;

namespace Booking.Models
{
    public class BookingSearchModel
    {
        [JsonProperty("bookingReferenceNo")]
        public string BookingReferenceNo { get; set; }
        [JsonProperty("surname")]
        public string Surname { get; set; }
    }
}
