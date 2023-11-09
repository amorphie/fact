using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;

public class SecurityImageModule
: BaseBBTRoute<SecurityImageDto, SecurityImage, UserDBContext>
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