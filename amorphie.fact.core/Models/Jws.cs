using amorphie.core.Base;

public class Jws : EntityBase
{
    public string? Mode { get; set; }
    public string? Header { get; set; }
    public string? Secret { get; set; }
    public string? Algorithm { get; set; }    
}