namespace amorphie.user;

public class DeactiveDefinitionModule
    : BaseDeactiveDefinitionModule<DeactiveDefinitionDto, DeactiveDefinition, DeactiveDefinitionValidator>
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