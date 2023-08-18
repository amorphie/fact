using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public class UserDeviceModule 
: BaseBBTRoute<UserDeviceDto, UserDevice, UserDBContext>
{
    public UserDeviceModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "ClientId", "DeviceId", "TokenId", "UserId" };

    public override string? UrlFragment => "userDevice";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapGet("search", getAllUserDeviceFullTextSearch);
    }

    async ValueTask<IResult> getAllUserDeviceFullTextSearch(
     [FromServices] UserDBContext context,
   [AsParameters] SecurityQuestionSearch dataSearch
    )
    {
        var query = context!.UserDevices!
            .Skip(dataSearch.Page * dataSearch.PageSize)
            .Take(dataSearch.PageSize);

        if (!string.IsNullOrEmpty(dataSearch.Keyword))
        {
            query = query.AsNoTracking().Where(x => EF.Functions.ToTsVector("english", string.Join(" ", x.DeviceId, x.UserId, x.Id, x.ClientId))
           .Matches(EF.Functions.PlainToTsQuery("english", dataSearch.Keyword)));
        }

        var userDevices = query.ToList();

        if (userDevices.Count() > 0)
        {
            var response = userDevices.Select(x => ObjectMapper.Mapper.Map<UserDeviceDto>(x)).ToList();
            return Results.Ok(response);
        }

        return Results.NotFound();
    }
}