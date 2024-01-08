using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class ClientAudience : EntityBase
{
    [ForeignKey("Client")]
    public Guid ClientId { get; set; }
    public string Name { get; set; } = default!;
}