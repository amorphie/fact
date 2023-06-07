using amorphie.user;

public class SecurityImageModule : BaseSecurityImageModule<SecurityImageDto, SecurityImage, SecurityImageValidator>
{
    public SecurityImageModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "Image" };

    public override string? UrlFragment => "securityImage";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
         base.AddRoutes(routeGroupBuilder);
    }    
}