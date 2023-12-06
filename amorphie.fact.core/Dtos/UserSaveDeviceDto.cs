using amorphie.core.Base;

public class UserSaveDeviceDto
{
    public Guid DeviceId { get; set; }
    public Guid InstallationId{get;set;}
    public string? DeviceToken{get;set;}
    public string? DeviceModel{get;set;}
    public string? DevicePlatform{get;set;}
}