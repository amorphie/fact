using amorphie.core.Base;

public class UserSecurityImageDto:DtoBase
{
    public string SecurityImage { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
