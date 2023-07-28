using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.fact.data;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

public static class  ZeebeUserDevice
    {
         public static void MapZeebeUserDeviceEndpoints(this WebApplication app)
    {
            app.MapPost("/user-device-control", userDeviceControl)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Maps user-device-control service worker on Zeebe";
                operation.Tags = new List<OpenApiTag> { new() { Name = "Zeebe" } };
                return operation;
            });
            
    }
       static IResult userDeviceControl(
           [FromBody] dynamic body,
          [FromServices] UserDBContext dbContext,
           HttpRequest request,
           HttpContext httpContext,
           [FromServices] DaprClient client
           ,IConfiguration configuration
       )
    {
        var transitionName = body.GetProperty("LastTransition").ToString();
        var instanceIdAsString = body.GetProperty("InstanceId").ToString();
        var data = body.GetProperty($"TRX-{transitionName}").GetProperty("Data");
        var recordIdAsString = body.GetProperty("RecordId").ToString();
        string triggeredByAsString = body.GetProperty($"TRX-{transitionName}").GetProperty("TriggeredBy").ToString();
        string triggeredByBehalfOfAsString = body.GetProperty($"TRX-{transitionName}").GetProperty("TriggeredByBehalfOf").ToString();
        Guid instanceId;
        if (!Guid.TryParse(instanceIdAsString, out instanceId))
        {
            return Results.BadRequest("InstanceId not provided or not as a GUID");
        }
        Guid recordId;
        if (!Guid.TryParse(recordIdAsString, out recordId))
        {
            return Results.BadRequest("RecordId not provided or not as a GUID");
        }
           Guid triggeredBy;
        if (!Guid.TryParse(triggeredByAsString, out triggeredBy))
        {
            return Results.BadRequest("triggeredBy not provided or not as a GUID");
        }
            Guid triggeredByBehalfOf;
        if (!Guid.TryParse(triggeredByBehalfOfAsString, out triggeredByBehalfOf))
        {
            return Results.BadRequest("triggeredBy not provided or not as a GUID");
        }
        try
        {
            dynamic? entityData=body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData");
            string reference = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("reference").ToString();
            string deviceId = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("deviceId").ToString();
            UserDevice? user = dbContext.UserDevices!.Include(i=>i.Users).FirstOrDefault(f => f.Users!.Reference==reference&&f.TokenId.ToString()==deviceId);
            if (user != null)
            {
                return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, true,"Success"));
              
            }
            else
            {
                 transitionName = "user-device-control-fail";
            return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false,"User Device Not Found"));
            }

        }
        catch (Exception ex)
        {
            transitionName = "user-device-control-fail";
            return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false,"Error User Device Searching"));
        }
        transitionName = "user-device-control-fail";
        return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false,"Error User Device Searching"));
    }
}