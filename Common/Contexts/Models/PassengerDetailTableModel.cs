using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contexts.Models
{
    [Table("PassengerDetails")]
    public class PassengerDetailTableModel : BaseModel
    {
        [Column("Name")]
        public string Name { get; set; }
        [Column("Surname")]
        public string Surname { get; set; }
        [Column("DateOfBirth")]
        public string DateOfBirth { get; set; }
        [Column("BookingReferenceNo")]
        public string BookingReferenceNo { get; set; }
        [Column("PassportIssuer")]
        public string PassportIssuer { get; set; }
        [Column("PassportNo")]
        public string PassportNo { get; set; }
    }
}
