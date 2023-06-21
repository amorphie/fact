using amorphie.core.Base;

public class JwsDto : DtoBase
{
    public string? Mode { get; set; }
    public string? Header { get; set; }
    public string? Secret { get; set; }
    public string? Algorithm { get; set; }
}

public class JwsGetDto
{
    public string? Mode { get; set; }
    public string? Header { get; set; }
    public string? Secret { get; set; }
    public string? Algorithm { get; set; }
}