using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace amorphie.user.data;

public class UserDevice:BaseEntity
{
    public int DeviceId { get; set; }
    public Guid TokenId { get; set; }
    public Guid ClientId { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    public User? Users { get; set; }

}