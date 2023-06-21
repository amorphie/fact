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

public class VariantGetDto
{
    public VariantGetDto(string? mode)
    {
        Mode = mode;
    }

    public string? Mode { get; set; }
}

public class SessionGetDto
{
    public SessionGetDto(string? header)
    {
        Header = header;
    }

    public string? Header { get; set; }
}

public class LocationGetDto
{
    public LocationGetDto(string? header)
    {
        Header = header;
    }

    public string? Header { get; set; }
}