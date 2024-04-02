using amorphie.core.Base;

public class UserSecurityImageDto:DtoBase
{
    public Guid SecurityImageId { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
