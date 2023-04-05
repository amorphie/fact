using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class UserSecurityImage:EntityBase
{
    public string SecurityImage { get; set; } = string.Empty;

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }




}