using amorphie.core.Module.minimal_api;
using amorphie.fact.data;

namespace amorphie.user;

public class DeactiveDefinitionModule
: BaseBBTRoute<DeactiveDefinitionDto, DeactiveDefinition, UserDBContext>
{
    public DeactiveDefinitionModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "Name" };

    public override string? UrlFragment => "deactiveDefinition";


    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);
    }

}