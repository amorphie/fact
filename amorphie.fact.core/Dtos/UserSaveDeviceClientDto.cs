using amorphie.core.Base;

public class UserSaveDeviceClientDto
{
    public string ClientId { get; set; }
    public Guid UserId { get; set; }
    public string DeviceId { get; set; }
    public Guid InstallationId { get; set; }
}