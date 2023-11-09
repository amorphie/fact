using amorphie.core.Base;

public class UserDeviceDto : DtoBase
{
    public int DeviceId { get; set; }
    public Guid TokenId { get; set; }
    public Guid ClientId { get; set; }
    public Guid UserId { get; set; }
    public User? Users { get; set; }
    public int Status { get; set; }
}