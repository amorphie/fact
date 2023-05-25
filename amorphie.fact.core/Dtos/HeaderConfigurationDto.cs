using amorphie.core.Base;

public class HeaderConfigurationDto: DtoBase
{
    public string? AccessToken { get; set; }
    public string? Variant { get; set; }
    public string? SessionId { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Ip { get; set; }    
    public string? Location { get; set; }
}