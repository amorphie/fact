namespace amorphie.user.data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class UserTag
{
    [Key]
    public Guid Id { get; set; }
    public string Tag { get; set; } = string.Empty;

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid CretedByBehalfOf { get; set; }
    public Guid ModifiedByBehalof { get; set; }
    public User? User { get; set; }
}