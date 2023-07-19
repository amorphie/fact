using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using amorphie.core.Base;
using NpgsqlTypes;

public class ClientDto : DtoBase
{
    public ICollection<MultilanguageText> Names { get; set; } = default!;
    public string[]? Tags { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Validations { get; set; }
    public string? Secret { get; set; }
    public string? ReturnUrl { get; set; }
    public string? LoginUrl { get; set; }
    public string? LogoutUrl { get; set; }
    public string? Pkce { get; set; }
    public HeaderConfigurationDto? HeaderConfig { get; set; }
    public Jws? Jws { get; set; }
    public Idempotency? Idempotency { get; set; }
    public ICollection<ClientToken> Tokens { get; set; } = default!;
    public ICollection<ClientGrantType> AllowedGrantTypes { get; set; } = default!;
    public ICollection<ClientFlow> Flows { get; set; } = default!;
}

public class ClientGetDto
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public List<MultilanguageText> Names { get; set; } = default!;

    public string? Name
    {
        get
        {
            if (Names != null && Names.Count > 0)
            {
                return Names[0].Label.ToString();
            }

            return "";
        }
    }

    public ICollection<ClientFlowGetDto> Flows { get; set; } = default!;

    [JsonPropertyName("allowedGrantTypes")]
    public ICollection<ClientGrantTypeGetDto> AllowedGrantTypes { get; set; } = default!;

    [JsonPropertyName("allowedScopeTags")]
    public string[]? Tags { get; set; }

    [JsonPropertyName("loginUrl")]
    public string? LoginUrl { get; set; }

    [JsonPropertyName("returnUrl")]
    public string? ReturnUrl { get; set; }

    [JsonPropertyName("logoutUrl")]
    public string? LogoutUrl { get; set; }

    [JsonPropertyName("clientSecret")]
    public string? Secret { get; set; }

    public string? Pkce { get; set; }

    public JwsGetDto? Jws { get; set; }

    public IdempotencyGetDto? Idempotency { get; set; }

    public VariantGetDto? Variant
    {
        get
        {
            if (HeaderConfig != null)
            {
                return new VariantGetDto(HeaderConfig.Variant);
            }

            return null;
        }
    }

    public SessionGetDto? Session
    {
        get
        {
            if (HeaderConfig != null)
            {
                return new SessionGetDto(HeaderConfig.SessionId);
            }

            return null;
        }
    }

    public LocationGetDto? Location
    {
        get
        {
            if (HeaderConfig != null)
            {
                return new LocationGetDto(HeaderConfig.Location);
            }

            return null;
        }
    }

    public ICollection<ClientTokenGetDto> Tokens { get; set; } = default!;

    [JsonIgnore]
    public HeaderConfigurationDto? HeaderConfig { get; set; }


    [JsonIgnore]
    public string? Validations { get; set; }

    [JsonIgnore]
    public string? Status { get; set; }

    [JsonIgnore]
    public string? Type { get; set; }

}

public class ValidateClientRequest
{
    public Guid ClientId { get; set; }
    public string Secret { get; set; } = default!;
}


public class ClientSearch : DtoSearchBase
{

}