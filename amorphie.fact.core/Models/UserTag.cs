using amorphie.core.Base;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;
public class UserTag:EntityBase
{
    public string Tag { get; set; } = string.Empty;
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}