using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;
using NpgsqlTypes;

public class Client : EntityBase
{
    public ICollection<Translation> Names { get; set; } = default!;
    public string[]? Tags { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Validations { get; set; }
    public string? Secret { get; set; }
    public string? ReturnUrl { get; set; }
    public string? LoginUrl { get; set; }
    public string? LogoutUrl { get; set; }
    public string? Pkce { get; set; }
    public HeaderConfiguration? HeaderConfig { get; set; }
    public Jws? Jws { get; set; }
    public Idempotency? Idempotency { get; set; }
    public ICollection<ClientToken> Tokens { get; set; } = default!;
    public ICollection<ClientGrantType> AllowedGrantTypes { get; set; } = default!;
    public ICollection<ClientFlow> Flows { get; set; } = default!;

    [NotMapped]
    public virtual NpgsqlTsVector SearchVector { get; set; }

    public string? JwtSecretSalt { get; set; }
}