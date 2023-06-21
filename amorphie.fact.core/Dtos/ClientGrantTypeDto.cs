using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class ClientGrantTypeDto : DtoBase
{
    public Guid ClientId { get; set; }
    public string GrantType { get; set; } = default!;
}

public class ClientGrantTypeGetDto 
{
    public string GrantType { get; set; } = default!;
}