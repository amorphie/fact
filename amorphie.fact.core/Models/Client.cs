using amorphie.core.Base;

public class Client : EntityBase
{
    public ICollection<Translation> Names { get; set; } = default!;
    public string[]? Tags { get; set; }
    public string? Status { get; set; }
    public ClientType Type { get; set; }
    public string? Validations { get; set; }
    public string[]? AvailableFlows { get; set; }
    public string? Secret { get; set; }
    public string? ReturnUrl { get; set; }
    public string? LoginUrl { get; set; }
    public string? LogoutUrl { get; set; }
    public string? Pkce { get; set; }
    public HeaderConfiguration? HeaderConfig { get; set; }
    public Jws? Jws { get; set; }
    public Idempotency? Idempotency { get; set; }
    public ICollection<ClientToken> Tokens { get; set; } = default!;
}

public enum ClientType : byte
{
    APPLICATION,
    GATEWAY
}