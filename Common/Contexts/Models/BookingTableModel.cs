using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Contexts.Models
{
    [Table("Bookings")]
    public class BookingTableModel : BaseModel
    {
        [Column("OutboundFlightFK")]
        public int OutboundFlightFK { get; set; }
        [Column("ReturnFlightFK")]
        public int? ReturnFlightFK { get; set; }
        [Column("FareClass")]
        public int FareClass { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("BookingReferenceNo")]
        public string BookingReferenceNo { get; set; }
    }
}
