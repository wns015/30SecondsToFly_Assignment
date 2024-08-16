using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Contexts.Models
{
    [Table("Bookings")]
    public class BookingTableModel : BaseModel
    {
        [Column("Name")]
        public string Name { get; set; }
        [Column("Surname")]
        public string Surname { get; set; }
        [Column("DateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [Column("BookingReference")]
        public string BookingReference { get; set; }
        [Column("PassportCountry")]
        public string PassportCountry { get; set; }
        [Column("PassportNo")]
        public string PassportNo { get; set; }
        [Column("OutboundFlightFK")]
        public int OutboundFlightFK { get; set; }
        [Column("ReturnFlightFK")]
        public int? ReturnFlightFK { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [Column("FareClass")]
        public string FareClass { get; set; }
    }
}
