using System.Net.Mime;
using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class UserDevicePublic
{
    public static async Task<IResult> saveDevice(
         [FromServices] UserDBContext context,
         [FromRoute(Name = "clientCode")] string clientCode,
      [FromBody] UserSaveDeviceDto deviceInfo
     )
    {
        var device = await context!.UserDevices
            .Where(u => u.DeviceId == deviceInfo.DeviceId && u.InstallationId == deviceInfo.InstallationId)
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
                ClientId = clientCode,
                UserId = null,
                TokenId = null,
                Status = 1
            });
            await context!.SaveChangesAsync();
        }

        return Results.Ok();
    }

    public static async Task<IResult> removeDevice(
         [FromServices] UserDBContext context,
      [FromRoute(Name = "clientCode")] string ClientCode,
      [FromRoute(Name = "reference")] string reference
     )
    {
        var user = await context.Users.OrderByDescending(u => u.CreatedAt).FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if(user == null)
            return Results.NotFound("User Not Found");
        
        var userDevice = await context.UserDevices.OrderByDescending(d => d.CreatedAt).FirstOrDefaultAsync(d => d.ClientId.Equals(ClientCode) && d.UserId.Equals(user.Id));
        if(userDevice == null)
            return Results.NotFound("User Has No Active Device");
        
        if(userDevice.Status == 1)
        {
            userDevice.ActivationRemovalDate = DateTime.UtcNow;
            userDevice.Status = 0;
            await context.SaveChangesAsync();
        }

        return Results.NoContent();
    }



    public static async Task<IResult> GetActiveDevice(
         [FromServices] UserDBContext context,
      [FromRoute(Name = "clientCode")] string ClientCode,
      [FromRoute(Name = "reference")] string reference
     )
    {   
        var user = await context.Users.OrderByDescending(u => u.CreatedAt).FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if(user == null)
            return Results.NotFound("User Not Found");
        
        var userDevice = await context.UserDevices.OrderByDescending(d => d.CreatedAt).FirstOrDefaultAsync(d => d.ClientId.Equals(ClientCode) && d.UserId.Equals(user.Id));
        if(userDevice == null)
            return Results.NotFound("User Has No Active Device");
        
        return Results.Ok(new ActiveDeviceDto{
            Id = userDevice.Id,
            ActivationRemovalDate = userDevice.ActivationRemovalDate,
            LastLogonDate = userDevice.LastLogonDate,
            RegistrationDate = userDevice.CreatedAt,
            DeviceId = userDevice.DeviceId, 
            Model = userDevice.DeviceModel,
            Platform = userDevice.DevicePlatform,
            CreatedByUserName = userDevice.CreatedBy.ToString(),
            Status = userDevice.Status == 1 ? 10 : 20
        });
    }

    public static async Task<IResult> RemoveDevice(
         [FromServices] UserDBContext context,
      [FromRoute(Name = "clientCode")] string ClientCode,
      [FromRoute(Name = "reference")] string reference
     )
    {   
        var user = await context.Users.OrderByDescending(u => u.CreatedAt).FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if(user == null)
            return Results.NotFound("User Not Found");
        
        var userDevice = await context.UserDevices.OrderByDescending(d => d.CreatedAt).FirstOrDefaultAsync(d => d.ClientId.Equals(ClientCode) && d.UserId.Equals(user.Id));
        if(userDevice == null)
            return Results.NotFound("User Has No Active Device");
        
        if(userDevice.Status == 1)
        {
            userDevice.Status = 0;
            await context.SaveChangesAsync();
        }

        return Results.NoContent();
    }
}