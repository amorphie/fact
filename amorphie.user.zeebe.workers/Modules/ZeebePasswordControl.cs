using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.fact.data;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

public static class ZeebePasswordControl
{
    public static void MapZeebeUserPasswordControlEndpoints(this WebApplication app)
    {
        app.MapPost("/user-password-control", userPasswordControl)
        .Produces(StatusCodes.Status200OK)
        .WithOpenApi(operation =>
        {
            operation.Summary = "Maps user-password-control service worker on Zeebe";
            operation.Tags = new List<OpenApiTag> { new() { Name = "Zeebe" } };
            return operation;
        });

    }
    static IResult userPasswordControl(
        [FromBody] dynamic body,
       [FromServices] UserDBContext dbContext,
        HttpRequest request,
        HttpContext httpContext,
        [FromServices] DaprClient client
        , IConfiguration configuration
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
            dynamic? entityData = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData");
            string reference = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("reference").ToString();
            string password = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("password").ToString();
            User? user = dbContext.Users!.Include(i => i.UserPasswords).FirstOrDefault(f => f.Reference == reference);

        if (user != null)
        {
            if (user.UserPasswords != null && user.UserPasswords.Count() > 0)
            {
                var userPassword = user.UserPasswords.Where(x => x.UserId == user.Id && x.IsArgonHash == true).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if (userPassword != null)
                {
                    var bytePassword = Convert.FromBase64String(userPassword.HashedPassword);
                    var salt = Convert.FromBase64String(user.Salt);
                    var checkPassword = ArgonPasswordHelper.VerifyHash(password, salt, bytePassword);

                    if (checkPassword)
                    {
                         return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, true, "Success"));
                    }

                   else{
                     transitionName = "user-password-control-fail";
                     return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false, "User Password does not match"));
                   }
                }
                else
                {
                     transitionName = "user-password-control-fail";
                      return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false, "User password is null"));
                }
            }
            else
            {
                 transitionName = "user-password-control-fail";
                 return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false, "User password is null"));
            }
        }
        else
        {
             transitionName = "user-password-control-fail";
            return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false, "User is not found"));
        }
        }
        catch (Exception ex)
        {
             transitionName = "user-password-control-fail";
            return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false, "Unexcepted error while User Password checking"));
        }
         transitionName = "user-password-control-fail";
        return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false, "Unexcepted error while User Password checking"));
    }
}