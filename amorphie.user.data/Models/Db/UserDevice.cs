using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class UserDevice
{
    [Key]
    public Guid Id { get; set; }

    public int DeviceId { get; set; }
    public Guid TokenId { get; set; }
    public Guid ClientId { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    public User? Users { get; set; }
     public Guid CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

    public Guid CretedByBehalfOf { get; set; }

    public Guid ModifiedByBehalof { get; set; }


}