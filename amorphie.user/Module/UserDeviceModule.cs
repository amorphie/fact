using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Http.HttpResults;
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
        routeGroupBuilder.MapPost("/public/save-device", saveDevice);
        routeGroupBuilder.MapPost("/save-device", saveDeviceClient);
    }

    async ValueTask<IResult> saveDeviceClient(
     [FromServices] UserDBContext context,
     [FromBody] UserSaveDeviceClientDto deviceInfo
    )
    {
        await context!.UserDevices.AddAsync(new UserDevice()
            {
                DeviceId = deviceInfo.DeviceId,
                InstallationId = deviceInfo.InstallationId,
                ClientId = deviceInfo.ClientId,
                UserId = deviceInfo.UserId,
                TokenId = null,
                 DeviceModel = null,
                  DevicePlatform = null,
                   DeviceToken = null,
                Status = 1
            });
            await context!.SaveChangesAsync();
            return Results.Ok();
    }

    async ValueTask<IResult> saveDevice(
     [FromServices] UserDBContext context,
     [FromBody] UserSaveDeviceDto deviceInfo
    )
    {
        var device = await context!.UserDevices
            .Where(u => u.DeviceId == deviceInfo.DeviceId && u.Status == 1)
            .FirstOrDefaultAsync();

        if(device == null)
        {
            await context!.UserDevices.AddAsync(new UserDevice()
            {
                DeviceId = deviceInfo.DeviceId,
                InstallationId = deviceInfo.InstallationId,
                DeviceModel = deviceInfo.DeviceModel,
                DevicePlatform = deviceInfo.DevicePlatform,
                DeviceToken = deviceInfo.DeviceToken,
                ClientId = null,
                UserId = null,
                TokenId = null,
                Status = 1
            });
            await context!.SaveChangesAsync();
            return Results.Ok();
        }

        if(device.InstallationId != deviceInfo.InstallationId)
        {
            device.Status = 0;

            await context!.UserDevices.AddAsync(new UserDevice()
            {
                DeviceId = deviceInfo.DeviceId,
                InstallationId = deviceInfo.InstallationId,
                DeviceModel = deviceInfo.DeviceModel,
                DevicePlatform = deviceInfo.DevicePlatform,
                DeviceToken = deviceInfo.DeviceToken,
                ClientId = device.ClientId,
                UserId = device.UserId,
                TokenId = device.TokenId,
                Status = 1
            });
            await context!.SaveChangesAsync();
            return Results.Ok();
        }

        if(!deviceInfo.DeviceToken!.Equals(device.DeviceToken))
        {
            device.DeviceToken = deviceInfo.DeviceToken;
            await context!.SaveChangesAsync();
            return Results.Ok();
        }

        return Results.Ok();
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
            query = query.AsNoTracking().Where(x => EF.Functions.ToTsVector("english", string.Join(" ", x.DeviceId, x.UserId, x.Id,x.ClientId))
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