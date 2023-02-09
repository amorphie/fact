using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class SecurityQuestion:BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public ICollection<UserSecurityQuestion>? UserSecurityQuestion { get; set; }

}