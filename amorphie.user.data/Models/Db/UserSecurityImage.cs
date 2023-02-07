using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class UserSecurityImage
{
    [Key]
    public Guid Id { get; set; }
    public string SecurityImage { get; set; } = string.Empty;

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

    public Guid CretedByBehalfOf { get; set; }

    public Guid ModifiedByBehalof { get; set; }
}