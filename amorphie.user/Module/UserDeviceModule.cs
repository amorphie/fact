using System.Text.Json.Serialization;
using amorphie.core.Module.minimal_api;
using amorphie.fact.core.Dtos.Device;
using amorphie.fact.core.Models;
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

        routeGroupBuilder.MapGet("/list/{reference}", getDeviceList);
        routeGroupBuilder.MapPut("/remove-activation/{reference}", removeDeviceActivation);
        routeGroupBuilder.MapGet("search", getAllUserDeviceFullTextSearch);
        //routeGroupBuilder.MapPost("/public/save-device", saveDevice);
        routeGroupBuilder.MapGet("/check-device-without-user/{clientId}/{deviceId}/{installationId}", checkDeviceWithoutUser);
        routeGroupBuilder.MapPost("/save-device", saveDeviceClient);
        routeGroupBuilder.MapPost("/save-mobile-device-client", saveMobileDeviceClient);
        routeGroupBuilder.MapPost("/save-mobile-device-client-init", saveMobileDeviceClientInit);
        routeGroupBuilder.MapGet("/check-device/{clientId}/{userId}/{deviceId}/{installationId}", checkDevice);
    }

    public static async Task<IResult> saveMobileDeviceClientInit(
         [FromServices] UserDBContext context,
         [FromHeader(Name = "clientIdReal")] string clientCode,
      [FromBody] UserSaveDeviceDto deviceInfo
     )
    {
        var device = await context!.UserDevices.FirstOrDefaultAsync(d => d.ClientId.Equals(clientCode) && d.DeviceId.Equals(deviceInfo.DeviceId));
        if(device is {})
        {
            if(device.InstallationId.Equals(deviceInfo.InstallationId))
            {
                device.Version = deviceInfo.DeviceVersion;
                if(!device.DeviceToken.Equals(deviceInfo.DeviceToken))
                {
                    device.DeviceToken = deviceInfo.DeviceToken;
                    await context!.SaveChangesAsync();
                }
            }
            else
            {
                device.Status = 0;
                var newDevice = new UserDevice
                {
                    ClientId = clientCode,
                    CreatedAt = DateTime.UtcNow,
                    DeviceId = deviceInfo.DeviceId,
                    DeviceModel = deviceInfo.DeviceModel,
                    InstallationId = deviceInfo.InstallationId,
                    DeviceToken = deviceInfo.DeviceToken,
                    DevicePlatform = deviceInfo.DevicePlatform,
                    Version = deviceInfo.DeviceVersion,
                    Status = 1
                };
                await context!.UserDevices.AddAsync(newDevice);
                await context!.SaveChangesAsync();
            }
        }
        else
        {
            device = new UserDevice
            {
                ClientId = clientCode,
                CreatedAt = DateTime.UtcNow,
                DeviceId = deviceInfo.DeviceId,
                DeviceModel = deviceInfo.DeviceModel,
                InstallationId = deviceInfo.InstallationId,
                DeviceToken = deviceInfo.DeviceToken,
                DevicePlatform = deviceInfo.DevicePlatform,
                Version = deviceInfo.DeviceVersion,
                Status = 1
            };
            await context!.UserDevices.AddAsync(device);
            await context!.SaveChangesAsync();
        }

        return Results.Ok();
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
        //removeAnotherDevicesBelongsToUser
        await context!.UserDevices.Where(d => !d.DeviceId.Equals(deviceInfo.DeviceId) && d.UserId.Equals(deviceInfo.UserId)).ExecuteUpdateAsync(s => s.SetProperty(d => d.Status, 0));

        var device = await context!.UserDevices
        .Where(d => d.ClientId.Equals(deviceInfo.ClientId) && d.DeviceId.Equals(deviceInfo.DeviceId) && d.InstallationId.Equals(deviceInfo.InstallationId) && d.Status == 1)
        .FirstOrDefaultAsync();

        if (device != null)
        {
            if (device.UserId != deviceInfo.UserId)
            {
                if (device.UserId != null)
                {
                    device.Status = 0;

                    await context!.UserDevices.AddAsync(new UserDevice()
                    {
                        DeviceId = deviceInfo.DeviceId,
                        InstallationId = deviceInfo.InstallationId,
                        DeviceToken = deviceInfo.DeviceToken,
                        DevicePlatform = deviceInfo.DevicePlatform,
                        DeviceModel = deviceInfo.DeviceModel,
                        Version = deviceInfo.DeviceVersion,
                        UserId = deviceInfo.UserId,
                        ClientId = deviceInfo.ClientId,
                        LastLogonDate = DateTime.UtcNow,
                        Status = 1
                    });
                }
                else
                {
                    device.Version = deviceInfo.DeviceVersion;
                    device.UserId = deviceInfo.UserId;
                    device.LastLogonDate = DateTime.UtcNow;
                }
            }
            else
            {
                device.Version = deviceInfo.DeviceVersion;
            }
            await context!.SaveChangesAsync();
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
                        Version = deviceInfo.DeviceVersion,
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
                    Version = deviceInfo.DeviceVersion,
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

    async ValueTask<IResult> getDeviceList(
     [FromServices] UserDBContext context,
     [FromRoute(Name = "reference")] string reference
    )
    {
        var user = await context!.Users.FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if (user == null)
        {
            return Results.NotFound("User Not Found");
        }

        var deviceList = await context!.UserDevices!.Where(u => u.UserId.Equals(user.Id) && u.Status == 1).ToListAsync();
        var response = new DeviceListDto();
        foreach (var device in deviceList)
        {
            if (device.IsRegistered)
            {
                response.Add(new DeviceInfo()
                {
                    DeviceId = device.DeviceId,
                    ActivationRemovalDate = device.ActivationRemovalDate,
                    CreatedByUserName = device.CreatedBy.ToString(),
                    Description = device.Description,
                    Id = device.Id,
                    LastLogonDate = device.LastLogonDate,
                    Manufacturer = device.Manufacturer,
                    Model = device.DeviceModel,
                    Platform = device.DevicePlatform,
                    RegistrationDate = device.RegistrationDate,
                    Status = DeviceStatusConstants.DeviceStatusMap[device.Status],
                    Version = device.Version
                });
            }
        }

        return Results.Json(response, new System.Text.Json.JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
    }

    async ValueTask<IResult> removeDeviceActivation(
     [FromServices] UserDBContext context,
     [FromRoute(Name = "reference")] string reference,
     [FromBody] RemoveDeviceActivationRequestDto removeDeviceActivationRequestDto
    )
    {
        var user = await context!.Users.FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if (user == null)
        {
            return Results.NotFound("User Not Found");
        }

        var device = await context!.UserDevices.FirstOrDefaultAsync(u => u.UserId.Equals(user.Id) && u.Id.Equals(removeDeviceActivationRequestDto.Id) && u.Status == 1);
        if (device is UserDevice && device.IsRegistered)
        {
            device.Status = 0;
            device.RemovalReason = removeDeviceActivationRequestDto.Description;
            device.ActivationRemovalDate = DateTime.UtcNow;
            device.ModifiedBy = user.Id;
            await context!.SaveChangesAsync();
        }

        return Results.Ok();
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
            var user = await context!.Users.FirstOrDefaultAsync(u => u.Id.Equals(device.UserId));
            return Results.Ok(new { Reference = user.Reference });
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