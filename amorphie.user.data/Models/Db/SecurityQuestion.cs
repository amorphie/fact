using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class SecurityQuestion
{
    [Key]
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid CretedByBehalfOf { get; set; }
    public Guid ModifiedByBehalof { get; set; }
    public ICollection<UserSecurityQuestion>? UserSecurityQuestion { get; set; }

}