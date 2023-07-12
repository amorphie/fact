using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using AutoMapper;
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
     [AsParameters] SecurityQuestionSearch dataSearch,
     [FromServices] IMapper mapper,
     CancellationToken cancellationToken
    )
    {
        var query = context!.UserDevices!
            .Skip(dataSearch.Page * dataSearch.PageSize)
            .Take(dataSearch.PageSize);

        if (!string.IsNullOrEmpty(dataSearch.Keyword))
        {
            query = query.AsNoTracking().Where(x => EF.Functions.ToTsVector("english", string.Join(" ", x.DeviceId, x.UserId, x.Id))
           .Matches(EF.Functions.PlainToTsQuery("english", dataSearch.Keyword)));
        }


        IList<UserDevice> resultList = await query.ToListAsync(cancellationToken);

        return (resultList != null && resultList.Count > 0)
            ? Results.Ok(mapper.Map<IList<UserDeviceDto>>(resultList))
            : Results.NoContent();
    }
}