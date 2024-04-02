using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class UserSecurityImage : EntityBase
{
    public SecurityImage SecurityImage { get; set; }
    [ForeignKey("SecurityImage")]
    public Guid SecurityImageId { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public bool? RequireChange { get; set; }
}