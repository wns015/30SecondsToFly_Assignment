using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Contexts.Models
{
    public class BaseModel
    {
        [Column("Id")]
        public int Id { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate = DateTime.Now;
    }
}
