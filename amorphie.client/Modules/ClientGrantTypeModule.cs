using amorphie.core.Module.minimal_api;
using amorphie.fact.data;

namespace amorphie.client;

public class ClientGrantTypeModule
    : BaseBBTRoute<ClientGrantTypeDto, ClientGrantType, UserDBContext>
{
    public ClientGrantTypeModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "ClientId", "GrantType" };

    public override string? UrlFragment => "clientGrantType";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);
    }
}