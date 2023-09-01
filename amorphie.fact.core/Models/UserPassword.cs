using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class UserPassword : EntityBase
{
    public string HashedPassword { get; set; }

    public int? AccessFailedCount { get; set; }

    public bool? MustResetPassword { get; set; }

    public bool IsArgonHash { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}