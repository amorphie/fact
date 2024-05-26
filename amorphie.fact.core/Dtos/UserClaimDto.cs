using amorphie.core.Base;

public class UserClaimDto : DtoBase
{
    public string ClaimName { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty; 
    public Guid UserId { get; set; }
}

public class UserClaimSearch : DtoSearchBase
{

}