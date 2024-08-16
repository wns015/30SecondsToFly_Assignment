using Newtonsoft.Json;

namespace Booking.Models
{
    public class PassengerModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("surname")]
        public string Surname { get; set; }
        [JsonProperty("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [JsonProperty("passportCountry")]
        public string PassportCountry { get; set; }
        [JsonProperty("passportNo")]
        public string PassportNo { get; set; }
    }
}
