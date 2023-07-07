using amorphie.core.Module.minimal_api;
using amorphie.fact.data;

namespace amorphie.client;

public class ClientTokenModule
    : BaseBBTRoute<ClientTokenDto, ClientToken, UserDBContext>
{
    public ClientTokenModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "Type" };

    public override string? UrlFragment => "clientToken";


    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);
    }

}