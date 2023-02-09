namespace amorphie.user.data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class UserTag:BaseEntity
{
    public string Tag { get; set; } = string.Empty;

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}