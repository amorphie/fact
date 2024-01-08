using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;

public class ClientAudienceDto : DtoBase
{   
    public Guid ClientId { get; set; }
    public string Name { get; set; } = default!;
}

public class ClientAudienceGetDto 
{
    public string Name { get; set; } = default!;
}