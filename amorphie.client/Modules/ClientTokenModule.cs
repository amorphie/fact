namespace amorphie.client;

public class ClientTokenModule
    : BaseClientTokenModule<ClientTokenDto, ClientToken, ClientTokenValidator>
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