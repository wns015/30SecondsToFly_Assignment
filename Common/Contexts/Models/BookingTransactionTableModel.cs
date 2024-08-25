using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Contexts.Models
{
    [Table("BookingTransactions")]
    public class BookingTransactionTableModel : BaseModel
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
        public bool TransactionStatus { get; set; }
    }
}
