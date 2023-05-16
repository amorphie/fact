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

public class SaveClientTokenRequest
{
    public Guid? Id { get; set; }
    public Guid ClientId { get; set; }
    public ClientTokenType Type { get; set; }
    public string? DefaultDuration { get; set; }
    public bool? OverrideDuration { get; set; }
    public string[]? PublicClaims { get; set; }
    public string[]? PrivateClaims { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? CreatedByBehalfOf { get; set; }
    public Guid ModifiedBy { get; set; }
    public Guid? ModifiedByBehalfOf { get; set; }
}