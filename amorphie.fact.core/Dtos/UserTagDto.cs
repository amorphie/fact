using amorphie.core.Base;

public class UserTagDto : DtoBase
{
    public string Tag { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User? User { get; set; }
}

public class UserTagSearch : DtoSearchBase
{

}