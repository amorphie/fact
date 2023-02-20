using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class UserDeviceModule
{
   
    static WebApplication _app = default!;

    public static void MapUserDeviceEndpoints(this WebApplication app)
    {
        _app = app;

         _app.MapGet("/userdevice", getAllUserDevice)
        .WithTags("UserDevice")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Returns saved userdevice records.";
            operation.Parameters[0].Description = "Filtering parameter. Given **userdevice** is used to filter userdevice.";
            operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
            return operation;
        })
        .Produces<GetUserDeviceResponse[]>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

        
         _app.MapGet("/userdevice/{userId}", getUserDevice)
        .WithTags("UserDevice")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Returns saved userdevice records.";
            operation.Parameters[0].Description = "Filtering parameter. Given **userdevice** is used to filter userdevice.";
            operation.Parameters[1].Description = "UserId of the requested device.";
            return operation;
        })
        .Produces<GetUserDeviceResponse[]>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);


       _app.MapPost("/userdevice", postUserDevice)
        .WithOpenApi()
        .WithSummary("Save userdevice")
        .WithDescription("")
        .WithTags("UserDevice")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

       _app.MapDelete("/userdevice/{id}", deleteUserDevice)
       .WithOpenApi()
       .WithSummary("Deletes userdevice")
       .WithDescription("Delete userdevice.")
       .WithTags("UserDevice")
       .Produces<GetUserDeviceResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

    }
      static IResult getAllUserDevice(
        [FromServices] UserDBContext context,
        [FromQuery] Guid ClientId,
        [FromQuery][Range(0, 100)] int page = 0,
        [FromQuery][Range(5, 100)] int pageSize = 100
        )
    {
        var query = context!.UserDevices!
            .Skip(page * pageSize)
            .Take(pageSize);

        if (ClientId!=null)
        {
           query=query.Where(t => t.ClientId==ClientId);
        }

        var userDevices = query.ToList();

        if (userDevices.Count() > 0)
        {
            return Results.Ok(userDevices.Select(userDevice =>
              new GetUserDeviceResponse(
               userDevice.Id,
               userDevice.DeviceId,
               userDevice.TokenId,
               userDevice.ClientId,
               userDevice.UserId,   
               userDevice.CreatedBy,
               userDevice.CreatedAt,
               userDevice.ModifiedBy,
               userDevice.ModifiedAt,
               userDevice.CretedByBehalfOf,
               userDevice.ModifiedByBehalof
               
                )
            ).ToArray());
        }
        else
            return Results.NoContent();
    }
     static IResult getUserDevice(
        [FromServices] UserDBContext context,
         [FromRoute(Name = "userId")] Guid userId,
        [FromQuery][Range(0, 100)] int page = 0,
        [FromQuery][Range(5, 100)] int pageSize = 100
        )
    {
        var query = context!.UserDevices!
            .Skip(page * pageSize)
            .Take(pageSize);

        
           var userDevices=query.Where(t => t.UserId==userId).ToList();
        

      
        if (userDevices.Count() > 0)
        {
              return Results.Ok(userDevices.Select(userDevice =>
               new GetUserDeviceResponse(
               userDevice.Id,
               userDevice.DeviceId,
               userDevice.TokenId,
               userDevice.ClientId,
               userDevice.UserId,   
               userDevice.CreatedBy,
               userDevice.CreatedAt,
               userDevice.ModifiedBy,
               userDevice.ModifiedAt,
               userDevice.CretedByBehalfOf,
               userDevice.ModifiedByBehalof
               
                )
            ).ToArray());
        }
        else
            return Results.NoContent();
    }
     static async Task<IResult> postUserDevice(
        [FromBody] PostUserDeviceRequest data,
        [FromServices] UserDBContext context
        )
    {
       
        var userDevice = context!.UserDevices!
          .FirstOrDefault(x => x.ClientId==data.ClientId );


        if (userDevice== null)
        {
            var newRecord = new UserDevice { 
            Id = Guid.NewGuid(), 
            DeviceId = data.DeviceId,
            UserId=data.UserId,
            TokenId=data.TokenId,
            ClientId=data.ClientId,
            CreatedAt = DateTime.Now,
            CreatedBy = data.CreatedBy,
            CretedByBehalfOf = data.CretedByBehalfOf
            };
            context!.UserDevices!.Add(newRecord);
            context.SaveChanges();
            return Results.Created($"/usertag/{data.UserId}", newRecord);
        }
        else{
              var hasChanges = false;
            // Apply update to only changed fields.
            if (data.ClientId != null && data.ClientId != userDevice.ClientId) {userDevice.ClientId=data.ClientId ; hasChanges = true; }
            if (data.DeviceId != 0 && data.DeviceId != userDevice.DeviceId) {userDevice.DeviceId=data.DeviceId ; hasChanges = true; }
            if (data.TokenId != null && data.TokenId != userDevice.TokenId) {userDevice.TokenId=data.TokenId ; hasChanges = true; }
            if (data.UserId != null && data.UserId != userDevice.UserId) {userDevice.UserId=data.UserId ; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != userDevice.ModifiedByBehalof) { userDevice.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != userDevice.ModifiedBy) { userDevice.ModifiedBy = data.ModifiedBy; hasChanges = true; }
                userDevice.ModifiedAt=DateTime.Now;
 
            if (hasChanges)
            {
                context!.SaveChanges();
                return Results.Ok(data);
            }
            else
            {
                return Results.Problem("Not Modified.", null, 304);
            }
        
        }
        return Results.Conflict("Request  is already used for another record.");
    }
     static IResult deleteUserDevice(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.UserDevices?.FirstOrDefault(t => t.Id == id);

        if (recordToDelete == null)
        {
            return Results.NotFound();
        }
        else
        {
            context!.Remove(recordToDelete);
            context.SaveChanges();
            return Results.Ok();
        }
    }
    }
