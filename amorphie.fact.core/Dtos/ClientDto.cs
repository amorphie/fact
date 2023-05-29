using amorphie.core.Base;

public class ClientDto : DtoBase
{
    public ICollection<MultilanguageText> Names { get; set; } = default!;
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
    public HeaderConfigurationDto? HeaderConfig { get; set; }
    public Jws? Jws { get; set; }
    public Idempotency? Idempotency { get; set; }
    public ICollection<ClientToken> Tokens { get; set; } = default!;
}

public class ValidateClientRequest
{
    public Guid ClientId { get; set; }
    public string Secret { get; set; } = default!;
    public string? ReturnUrl { get; set; }
}