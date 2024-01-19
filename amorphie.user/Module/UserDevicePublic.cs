using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class UserDevicePublic
{
    public static async Task<IResult> saveDevice(
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

    public static async Task<IResult> removeDevice(
         [FromServices] UserDBContext context,
      [FromRoute(Name = "clientCode")] string ClientCode,
      [FromRoute(Name = "reference")] string reference
     )
    {
        var user = await context.Users.OrderByDescending(u=> u.CreatedAt).FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if(user == null)
            return Results.NotFound("User Not Found");

        await context.UserDevices.Where(d => d.ClientId.Equals(ClientCode) && d.UserId.Equals(user.Id) && d.Status == 1).ExecuteUpdateAsync(s => s.SetProperty(d=>d.Status,0));
        return Results.Ok();
    }
}