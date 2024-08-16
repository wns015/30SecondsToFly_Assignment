using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Contexts.Models
{
    [Table("Flights")]
    public class FlightTableModel : BaseModel
    {
        [Column("Origin")]
        public string Origin { get; set; }
        [Column("Destination")]
        public string Destination { get; set; }
        [Column("Airline")]
        public string Airline { get; set; }
        [Column("FlightNo")]
        public string FlightNo { get; set; }
        [Column("Duration")]
        public int Duration { get; set; }
        [Column("DepartureTime")]
        public DateTime DepartureTime { get; set; }
        [Column("ArrivalTime")]
        public DateTime ArrivalTime { get; set; }
        [Column("EconomySeating")]
        public int? EconomySeating { get; set; }
        [Column("EconomyFare")]
        public double? EconomyFare { get; set; }
        [Column("PremiumSeating")]
        public int? PremiumSeating { get; set; }
        [Column("PremiumFare")]
        public double? PremiumFare { get; set; }
        [Column("BusinessSeating")]
        public int? BusinessSeating { get; set; }
        [Column("BusinessFare")]
        public double? BusinessFare { get; set; }
        [Column("FirstClassSeating")]
        public int? FirstClassSeating { get; set; }
        [Column("FirstClassFare")]
        public double? FirstClassFare { get; set; }
    }
}
