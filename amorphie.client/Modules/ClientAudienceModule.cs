using amorphie.core.Module.minimal_api;
using amorphie.fact.data;

namespace amorphie.client;

public class ClientAudienceModule
    : BaseBBTRoute<ClientAudienceDto, ClientAudience, UserDBContext>
{
    public ClientAudienceModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "ClientId", "Name" };

    public override string? UrlFragment => "clientAudience";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);
    }
}