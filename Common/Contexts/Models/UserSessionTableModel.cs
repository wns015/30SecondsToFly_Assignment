
using System.ComponentModel.DataAnnotations.Schema;
using Common.Contexts.Models;

[Table("UserSession")]
public class UserSessionTableModel : BaseModel {
    [Column("AccessToken")]
    public string AccessTaken { get; set; }
    [Column("TokenIssued")]
    public DateTime TokenIssued { get; set; }
    [Column("TokenExpiration")]
    public DateTime TokenExpiration { get; set; }
    [Column("IsActive")]
    public bool IsActive { get; set; }
}