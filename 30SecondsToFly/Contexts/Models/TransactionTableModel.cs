using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Contexts.Models
{
    [Table("Transactions")]
    public class TransactionTableModel : BaseModel
    {
        [Column("TransactionReferenceNo")]
        public string TransactionReferenceNo { get; set; }
        [Column("BookingReferenceNo")]
        public string BookingReferenceNo { get; set; }
        [Column("PaymentType")]
        public int PaymentType { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("TransactionStatus")]
        public string TransactionStatus { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
}
