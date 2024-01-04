using amorphie.core.Base;

public class UserSaveDeviceClientDto
{
    public Guid ClientId { get; set; }
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid InstallationId { get; set; }
}