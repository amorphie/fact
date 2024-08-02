using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserClaimModule 
: BaseBBTRoute<UserClaimDto, UserClaim, UserDBContext>
{

    public UserClaimModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "ClaimName","ClaimValue" };

    public override string? UrlFragment => "userClaim";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);
        routeGroupBuilder.MapGet("getByUserId/{userId}", GetClaimsBelongToUser);
    }

    async ValueTask<IResult> GetClaimsBelongToUser(
       [FromServices] UserDBContext context,
       [FromRoute(Name = "userId")] Guid userId
     )
    {
        var userClaims = await context.UserClaims!.Where(c => c.UserId == userId).ToListAsync();
        return Results.Ok(userClaims);
    }

}