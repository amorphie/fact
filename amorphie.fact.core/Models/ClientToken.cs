using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class ClientToken : EntityBase
{
    [ForeignKey("Client")]
    public Guid ClientId { get; set; }
    public ClientTokenType Type { get; set; }
    public string? DefaultDuration { get; set; }
    public bool? OverrideDuration{get;set;}
    public string[]? PublicClaims { get; set; }
    public string[]? PrivateClaims { get; set; }
}

public enum ClientTokenType : byte
{
    ACCESS,
    REFRESH,
    IDENTITY,
    FLOW
}