using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Mvc;

public static class UserDevicePublic 
{
   public static async Task<IResult> saveDevice(
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
}