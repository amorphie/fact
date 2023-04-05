using amorphie.core.Base;

public class ClientDto
{
    public ICollection<MultilanguageText> Names { get; set; } = default!;
    public string[]? Tags { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Validations { get; set; }
    public string[]? AvailableFlows { get; set; }
    public string? Secret { get; set; }
    public string? ReturnUrl { get; set; }
    public HeaderConfigurationDto? HeaderConfig { get; set; }
}

public class SaveClientRequest
{
    public Guid? Id { get; set; }
    public ICollection<MultilanguageText> Names { get; set; } = default!;
    public string[]? Tags { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Validations { get; set; }
    public string[]? AvailableFlows { get; set; }
    public string? Secret { get; set; }
    public string? ReturnUrl { get; set; }
    public HeaderConfigurationDto? HeaderConfig { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? CreatedByBehalfOf { get; set; }
    public Guid ModifiedBy { get; set; }
    public Guid? ModifiedByBehalfOf { get; set; }
}