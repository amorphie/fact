using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class ClientGrantType : EntityBase
{
    [ForeignKey("Client")]
    public Guid ClientId { get; set; }
    public string GrantType { get; set; } = default!;
}