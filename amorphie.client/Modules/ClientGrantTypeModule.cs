namespace amorphie.client;

public class ClientGrantTypeModule
    : BaseClientGrantTypeModule<ClientGrantTypeDto, ClientGrantType, ClientGrantTypeValidator>
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