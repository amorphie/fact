using amorphie.core.Base;

public class Client : EntityBase
{
    public ICollection<Translation> Names { get; set; } = default!;
    public string[]? Tags { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Validations { get; set; }
    public string[]? AvailableFlows { get; set; }
    public string? Secret { get; set; }
    public string? ReturnUrl { get; set; }
    public HeaderConfiguration? HeaderConfig { get; set; }
}