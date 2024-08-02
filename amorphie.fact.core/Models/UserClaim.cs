using amorphie.core.Base;
using System.ComponentModel.DataAnnotations.Schema;

public class UserClaim : EntityBase
{
    public string ClaimName { get; set; } = string.Empty;
    public string ClaimValue{get;set;} = string.Empty;
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}