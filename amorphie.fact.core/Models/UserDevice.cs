using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;



public class UserDevice:EntityBase
{
    public int DeviceId { get; set; }
    public Guid TokenId { get; set; }
    public Guid ClientId { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? Users { get; set; }

}