using amorphie.core.Module.minimal_api;
using amorphie.fact.core.Dtos.SecurityImage;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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