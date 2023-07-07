using amorphie.core.Module.minimal_api;
using amorphie.fact.data;

namespace amorphie.client;

public class ClientFlowModule
    : BaseBBTRoute<ClientDto, Client, UserDBContext>
{
    public ClientFlowModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "ClientId", "Type", "Workflow", "Token", "TokenDuration" };

    public override string? UrlFragment => "clientFlow";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);
    }
}