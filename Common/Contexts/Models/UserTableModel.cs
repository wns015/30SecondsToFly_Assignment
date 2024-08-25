

using System.ComponentModel.DataAnnotations.Schema;
using Common.Contexts.Models;

[Table("User")]
public class UserTableModel : BaseModel{
    [Column("Username")]
    public string Username { get; set; }
    [Column("Name")]
    public string Name { get; set; }
    [Column("Surname")]
    public string Surname { get; set; }
    [Column("Email")]
    public string Email { get; set; }
    [Column("RewardPoints")]
    public int RewardPoints { get; set; }
    [Column("Password")]
    public string Password { get; set; }
    [Column("Salt")]
    public string Salt { get; set; }
}