using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
using amorphie.fact.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public static class UserDeviceModule
{

    static WebApplication _app = default!;

    public static void MapUserDeviceEndpoints(this WebApplication app)
    {
        _app = app;

    //     _app.MapGet("/userdevice", getAllUserDevice)
    //    .WithTags("UserDevice")
    //    .WithOpenApi(operation =>
    //    {
    //        operation.Summary = "Returns saved userdevice records.";
    //        operation.Parameters[0].Description = "Filtering parameter. Given **userdevice** is used to filter userdevice.";
    //        operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
    //        return operation;
    //    })
    //    .Produces<GetUserDeviceResponse[]>(StatusCodes.Status200OK)
    //    .Produces(StatusCodes.Status204NoContent);

     _app.MapGet("/userdevice", getAllUserDeviceFullTextSearch)
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
    // static IResponse<List<GetUserDeviceResponse>> getAllUserDevice(
    //   [FromServices] UserDBContext context,
    //   [FromQuery] Guid ClientId,
    //   [FromQuery][Range(0, 100)] int page = 0,
    //   [FromQuery][Range(5, 100)] int pageSize = 100
    //   )
    // {
    //     var query = context!.UserDevices!
    //         .Skip(page * pageSize)
    //         .Take(pageSize);

    //     if (ClientId != null)
    //     {
    //         query = query.Where(t => t.ClientId == ClientId);
    //     }

    //     var userDevices = query.ToList();

    //     if (userDevices.Count() > 0)
    //     {
    //         return new Response<List<GetUserDeviceResponse>>
    //         {
    //             Data = userDevices.Select(x => ObjectMapper.Mapper.Map<GetUserDeviceResponse>(x)).ToList(),
    //             Result = new Result(Status.Success, "List return successfull")
    //         };
    //         // return Results.Ok(userDevices.Select(userDevice =>
    //         //   new GetUserDeviceResponse(
    //         //    userDevice.Id,
    //         //    userDevice.DeviceId,
    //         //    userDevice.TokenId,
    //         //    userDevice.ClientId,
    //         //    userDevice.UserId,   
    //         //    userDevice.CreatedBy,
    //         //    userDevice.CreatedAt,
    //         //    userDevice.ModifiedBy,
    //         //    userDevice.ModifiedAt,
    //         //    userDevice.CreatedByBehalfOf,
    //         //    userDevice.ModifiedByBehalfOf

    //         //     )
    //         // ).ToArray());
    //     }
    //     else
    //     {
    //         return new Response<List<GetUserDeviceResponse>>
    //         {
    //             Data = null,
    //             Result = new Result(Status.Success, "No content")
    //         };
    //     }
    // }
     static IResponse<List<GetUserDeviceResponse>> getAllUserDeviceFullTextSearch(
      [FromServices] UserDBContext context,
      [FromQuery] string? SearchText,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var query = context!.UserDevices!
            .Skip(page * pageSize)
            .Take(pageSize);

         if (!string.IsNullOrEmpty(SearchText))
        {
            query = query.Where(x => EF.Functions.ToTsVector("english",string.Join(" ",x.DeviceId,x.UserId,x.Id))
           .Matches(EF.Functions.PlainToTsQuery("english", SearchText)));
  
        }

        var userDevices = query.ToList();

        if (userDevices.Count() > 0)
        {
            return new Response<List<GetUserDeviceResponse>>
            {
                Data = userDevices.Select(x => ObjectMapper.Mapper.Map<GetUserDeviceResponse>(x)).ToList(),
                Result = new Result(Status.Success, "List return successfull")
            };
          
        }
        else
        {
            return new Response<List<GetUserDeviceResponse>>
            {
                Data = null,
                Result = new Result(Status.Success, "No content")
            };
        }
    }
    static IResponse<List<GetUserDeviceResponse>> getUserDevice(
       [FromServices] UserDBContext context,
        [FromRoute(Name = "userId")] Guid userId,
       [FromQuery][Range(0, 100)] int page = 0,
       [FromQuery][Range(5, 100)] int pageSize = 100
       )
    {
        var query = context!.UserDevices!
            .Skip(page * pageSize)
            .Take(pageSize);


        var userDevices = query.Where(t => t.UserId == userId).ToList();



        if (userDevices.Count() > 0)
        {
            return new Response<List<GetUserDeviceResponse>>
            {
                Data = userDevices.Select(x => ObjectMapper.Mapper.Map<GetUserDeviceResponse>(x)).ToList(),
                Result = new Result(Status.Success, "List return successfull")
            };
            //   return Results.Ok(userDevices.Select(userDevice =>
            //    new GetUserDeviceResponse(
            //    userDevice.Id,
            //    userDevice.DeviceId,
            //    userDevice.TokenId,
            //    userDevice.ClientId,
            //    userDevice.UserId,   
            //    userDevice.CreatedBy,
            //    userDevice.CreatedAt,
            //    userDevice.ModifiedBy,
            //    userDevice.ModifiedAt,
            //    userDevice.CreatedByBehalfOf,
            //    userDevice.ModifiedByBehalfOf

            //     )
            // ).ToArray());
        }
        else
        {
            return new Response<List<GetUserDeviceResponse>>
            {
                Data = null,
                Result = new Result(Status.Success, "No content")
            };
        }
    }
    static IResponse<GetUserDeviceResponse> postUserDevice(
       [FromBody] PostUserDeviceRequest data,
       [FromServices] UserDBContext context
       )
    {

        var userDevice = context!.UserDevices!
          .FirstOrDefault(x => x.ClientId == data.ClientId);


        if (userDevice == null)
        {
            var newRecord = ObjectMapper.Mapper.Map<UserDevice>(data);
            newRecord.CreatedAt = DateTime.UtcNow;
            // var newRecord = new UserDevice { 
            // Id = Guid.NewGuid(), 
            // DeviceId = data.DeviceId,
            // UserId=data.UserId,
            // TokenId=data.TokenId,
            // ClientId=data.ClientId,
            // CreatedAt = DateTime.Now,
            // CreatedBy = data.CreatedBy,
            // CreatedByBehalfOf = data.CreatedByBehalfOf
            // };
            context!.UserDevices!.Add(newRecord);
            context.SaveChanges();
            return new Response<GetUserDeviceResponse>
            {
                Data = ObjectMapper.Mapper.Map<GetUserDeviceResponse>(newRecord),
                Result = new Result(Status.Success, "Add successfull")
            };
        }
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.ClientId != null && data.ClientId != userDevice.ClientId) { userDevice.ClientId = data.ClientId; hasChanges = true; }
            if (data.DeviceId != 0 && data.DeviceId != userDevice.DeviceId) { userDevice.DeviceId = data.DeviceId; hasChanges = true; }
            if (data.TokenId != null && data.TokenId != userDevice.TokenId) { userDevice.TokenId = data.TokenId; hasChanges = true; }
            if (data.UserId != null && data.UserId != userDevice.UserId) { userDevice.UserId = data.UserId; hasChanges = true; }
            if (data.Status != null && data.Status != userDevice.Status) { userDevice.Status = data.Status; hasChanges = true; }
            if (data.ModifiedByBehaloff != null && data.ModifiedByBehaloff != userDevice.ModifiedByBehalfOf) { userDevice.ModifiedByBehalfOf = data.ModifiedByBehaloff; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != userDevice.ModifiedBy) { userDevice.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            userDevice.ModifiedAt = DateTime.Now;

            if (hasChanges)
            {
                context!.SaveChanges();
                return new Response<GetUserDeviceResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserDeviceResponse>(userDevice),
                    Result = new Result(Status.Success, "Update successfull")
                };
            }
            else
            {
                return new Response<GetUserDeviceResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserDeviceResponse>(userDevice),
                    Result = new Result(Status.Error, "Not modified")
                };
            }

        }
        return new Response<GetUserDeviceResponse>
        {
            Data = ObjectMapper.Mapper.Map<GetUserDeviceResponse>(userDevice),
            Result = new Result(Status.Error, "Request is already used for another record")
        };
    }
    static IResponse deleteUserDevice(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.UserDevices?.FirstOrDefault(t => t.Id == id);
        if (recordToDelete == null)
        {
            return new NoDataResponse
            {
                Result = new Result(Status.Success, "Device is not found")
            };
        }
        else
        {
            context!.Remove(recordToDelete);
            context.SaveChanges();
            return new NoDataResponse
            {
                Result = new Result(Status.Error, "Delete successful")
            };
        }
    }
}
