namespace Common.Contexts.Models
{
    public class BaseModel
    {
        [Column("Id")]
        public int Id { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; } = new DateTime.Now
    }
}
