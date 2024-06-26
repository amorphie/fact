using amorphie.core.Base;

public class UserSaveMobileDeviceDto
{
    public string DeviceId { get; set; }
    public Guid InstallationId { get; set; }
    public string? DeviceToken { get; set; }
    public string? DeviceModel { get; set; }
    public string? DevicePlatform { get; set; }
    public string? DeviceVersion{get;set;}
    public Guid UserId { get; set; }
    public string ClientId { get; set; }
}