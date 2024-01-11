using amorphie.core.Base;

public class UserDeviceDto : DtoBase
{
    public string DeviceId { get; set; }
    public Guid TokenId { get; set; }
    public string? ClientId { get; set; }
    public Guid UserId { get; set; }
    public User? Users { get; set; }
    public int Status { get; set; }
}