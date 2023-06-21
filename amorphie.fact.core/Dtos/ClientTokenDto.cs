using System.Text.Json.Serialization;
using amorphie.core.Base;

public class ClientTokenDto : DtoBase
{
    public Guid ClientId { get; set; }
    public ClientTokenType Type { get; set; }
    public string? DefaultDuration { get; set; }
    public bool? OverrideDuration { get; set; }
    public string[]? PublicClaims { get; set; }
    public string[]? PrivateClaims { get; set; }
}

public class ClientTokenGetDto 
{
    public ClientTokenType Type { get; set; }

    [JsonPropertyName("duration")]
    public string? DefaultDuration { get; set; }
    
    [JsonPropertyName("claims")]
    public string[]? PublicClaims { get; set; }
}