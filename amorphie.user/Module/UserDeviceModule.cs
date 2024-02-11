using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.fact.data.Migrations;
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
        //routeGroupBuilder.MapPost("/public/save-device", saveDevice);
        routeGroupBuilder.MapPost("/check-device/{clientId}/{deviceId}/{installationId}", checkDeviceWithoutUser);
        routeGroupBuilder.MapPost("/save-device", saveDeviceClient);
        routeGroupBuilder.MapPost("/save-mobile-device-client", saveMobileDeviceClient);
        routeGroupBuilder.MapGet("/check-device/{clientId}/{userId}/{deviceId}/{installationId}", checkDevice);
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

    async ValueTask<IResult> saveMobileDeviceClient(
     [FromServices] UserDBContext context,
     [FromBody] UserSaveMobileDeviceDto deviceInfo
    )
    {
        var device = await context!.UserDevices
        .Where(d => d.ClientId.Equals(deviceInfo.ClientId) && d.DeviceId.Equals(deviceInfo.DeviceId) && d.InstallationId.Equals(deviceInfo.InstallationId) && d.Status == 1)
        .FirstOrDefaultAsync();

        if (device != null)
        {
            if (device.UserId != deviceInfo.UserId)
            {
                if(device.UserId == null)
                {
                    device.Status = 0;

                    await context!.UserDevices.AddAsync(new UserDevice()
                    {
                        DeviceId = deviceInfo.DeviceId,
                        InstallationId = deviceInfo.InstallationId,
                        DeviceToken = deviceInfo.DeviceToken,
                        DevicePlatform = deviceInfo.DevicePlatform,
                        DeviceModel = deviceInfo.DeviceModel,
                        UserId = deviceInfo.UserId,
                        ClientId = deviceInfo.ClientId,
                        LastLogonDate = DateTime.UtcNow,
                        Status = 1
                    });
                }
                else
                {
                    device.UserId = deviceInfo.UserId;
                    device.LastLogonDate = DateTime.UtcNow;
                }
                await context!.SaveChangesAsync();
                return Results.Ok();
            }
            return Results.Ok();
        }
        else
        {
            device = await context!.UserDevices
            .Where(d => d.ClientId.Equals(deviceInfo.ClientId) && d.DeviceId.Equals(deviceInfo.DeviceId) && d.Status == 1)
            .FirstOrDefaultAsync();
            if (device != null)
            {
                if (device.InstallationId != deviceInfo.InstallationId)
                {
                    device.Status = 0;

                    await context!.UserDevices.AddAsync(new UserDevice()
                    {
                        DeviceId = deviceInfo.DeviceId,
                        InstallationId = deviceInfo.InstallationId,
                        DeviceToken = deviceInfo.DeviceToken,
                        DevicePlatform = deviceInfo.DevicePlatform,
                        DeviceModel = deviceInfo.DeviceModel,
                        UserId = deviceInfo.UserId,
                        ClientId = deviceInfo.ClientId,
                        LastLogonDate = DateTime.UtcNow,
                        Status = 1
                    });
                    await context!.SaveChangesAsync();
                    return Results.Ok();
                }
                return Results.Ok();
            }
            else
            {
                await context!.UserDevices.AddAsync(new UserDevice()
                {
                    DeviceId = deviceInfo.DeviceId,
                    InstallationId = deviceInfo.InstallationId,
                    DeviceToken = deviceInfo.DeviceToken,
                    DevicePlatform = deviceInfo.DevicePlatform,
                    DeviceModel = deviceInfo.DeviceModel,
                    UserId = deviceInfo.UserId,
                    ClientId = deviceInfo.ClientId,
                    LastLogonDate = DateTime.UtcNow,
                    Status = 1
                });
                await context!.SaveChangesAsync();
                return Results.Ok();
            }
        }

    }

    async ValueTask<IResult> checkDevice(
     [FromServices] UserDBContext context,
     [FromRoute(Name = "clientId")] string clientId,
     [FromRoute(Name = "userId")] Guid userId,
     [FromRoute(Name = "deviceId")] string deviceId,
     [FromRoute(Name = "installationId")] Guid installationId
    )
    {
        var device = await context!.UserDevices
            .Where(d => d.ClientId == clientId && d.UserId == userId && d.DeviceId == deviceId && d.InstallationId == installationId && d.Status == 1)
            .FirstOrDefaultAsync();

        if (device != null)
            return Results.Ok();
        else
            return Results.NotFound();
    }

    async ValueTask<IResult> checkDeviceWithoutUser(
     [FromServices] UserDBContext context,
     [FromRoute(Name = "clientId")] string clientId,
     [FromRoute(Name = "deviceId")] string deviceId,
     [FromRoute(Name = "installationId")] Guid installationId
    )
    {
        var device = await context!.UserDevices
            .Where(d => d.ClientId == clientId && d.DeviceId == deviceId && d.InstallationId == installationId && d.Status == 1)
            .FirstOrDefaultAsync();

        if (device != null)
        {
            var user = await context!.Users.FirstOrDefaultAsync(u => u.Id.Equals(device.Id));
            return Results.Ok(new{Reference=user.Reference});
        }
        else
            return Results.NotFound();
    }

    async ValueTask<IResult> saveDevice(
     [FromServices] UserDBContext context,
     [FromBody] UserSaveDeviceDto deviceInfo
    )
    {
        var device = await context!.UserDevices
            .Where(u => u.DeviceId == deviceInfo.DeviceId && u.Status == 1)
            .FirstOrDefaultAsync();

        if (device == null)
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

        if (device.InstallationId != deviceInfo.InstallationId)
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

        if (!deviceInfo.DeviceToken!.Equals(device.DeviceToken))
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