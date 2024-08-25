
using System.ComponentModel.DataAnnotations.Schema;
using Common.Contexts.Models;

[Table("UserSession")]
public class UserSessionTableModel : BaseModel {
    [Column("Username")]
    public string Username { get; set; }
    [Column("AccessToken")]
    public string AccessToken { get; set; }
    [Column("TokenIssued")]
    public DateTime TokenIssued { get; set; }
    [Column("TokenExpiration")]
    public DateTime TokenExpiration { get; set; }
    [Column("IsActive")]
    public bool IsActive { get; set; }
}