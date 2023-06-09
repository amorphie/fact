using amorphie.core.Base;

public class ClientFlowDto : DtoBase
{
    public Guid ClientId { get; set; }
    public string Type { get; set; } = default!;
    public string Workflow { get; set; } = default!;
    public ClientTokenType Token { get; set; } = default!;
    public string TokenDuration { get; set; } = default!;
}