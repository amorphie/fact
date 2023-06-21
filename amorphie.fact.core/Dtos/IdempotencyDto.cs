using amorphie.core.Base;

public class IdempotencyDto : DtoBase
{
    public string? Mode { get; set; }
    public string? Header { get; set; }
}

public class IdempotencyGetDto
{
    public string? Mode { get; set; }
    public string? Header { get; set; }
}