using amorphie.core.Base;

public class SecurityQuestion : EntityBase
{
    public string Question { get; set; } = string.Empty;
    public string? Key { get; set; }
    public string? ValueTypeClr { get; set; }
    public int? Priority { get; set; }
    public bool? IsActive { get; set; }
    public string? DescriptionTr { get; set; }
    public string? DescriptionEn { get; set; }
    public ICollection<UserSecurityQuestion>? UserSecurityQuestion { get; set; }
}