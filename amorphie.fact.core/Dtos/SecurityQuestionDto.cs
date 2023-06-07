using amorphie.core.Base;

public class SecurityQuestionDto : DtoBase
{
    public string Question { get; set; } = string.Empty;
    public ICollection<UserSecurityQuestion>? UserSecurityQuestion { get; set; }
}

public class SecurityQuestionSearch : DtoSearchBase
{

}