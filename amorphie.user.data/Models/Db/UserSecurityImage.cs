using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class UserSecurityImage:BaseEntity
{
    public string SecurityImage { get; set; } = string.Empty;

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}