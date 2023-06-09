using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class ClientFlow : EntityBase
{
    [ForeignKey("Client")]
    public Guid ClientId { get; set; }
    public string Type { get; set; } = default!;
    public string Workflow { get; set; } = default!;
    public ClientTokenType Token { get; set; } = default!;
    public string TokenDuration { get; set; } = default!;
}
