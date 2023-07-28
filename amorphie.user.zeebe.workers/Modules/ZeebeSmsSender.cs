using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.fact.data;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net.Http.Json;
using Konscious.Security.Cryptography;

public static class ZeebeSmsSender
{
    public static void MapZeebeSmsSenderEndpoints(this WebApplication app)
    {
        app.MapPost("/amorphie-user-send-password-sms", postSmsSender)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Maps amorphie-user-send-password-sms service worker on Zeebe";
                operation.Tags = new List<OpenApiTag> { new() { Name = "Zeebe" } };
                return operation;
            });
            app.MapPost("/amorphie-user-send-sms-key", postSmsKeySender)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Maps amorphie-user-send-sms-key service worker on Zeebe";
                operation.Tags = new List<OpenApiTag> { new() { Name = "Zeebe" } };
                return operation;
            });
            app.MapPost("/amorphie-sms-control", postSmsKeyControl)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Maps amorphie-user-send-sms-key service worker on Zeebe";
                operation.Tags = new List<OpenApiTag> { new() { Name = "Zeebe" } };
                return operation;
            });
    }
    static IResult postSmsSender(
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
        var triggeredBy = body.GetProperty($"TRX-{transitionName}").GetProperty("TriggeredBy").ToString();
        var triggeredByBehalfOf = body.GetProperty($"TRX-{transitionName}").GetProperty("TriggeredByBehalfOf").ToString();
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
        try
        {
            User? user = dbContext.Users!.FirstOrDefault(f => f.Id == recordId);
            if (user != null)
            {
                string password = Guid.NewGuid().ToString().Substring(0, 6);
                string content = "Yeni şifreniz:" + password;
                bool smsSend = SendSms(content, user,configuration,triggeredByBehalfOf);
                if (smsSend)
                {
                    var salt = Convert.FromBase64String(user.Salt);
                    var passwordHash = ArgonPasswordHelper.HashPassword(password, salt);

                    dbContext.UserPasswords!.Add(new UserPassword
                    {
                        Id = new Guid(),
                        HashedPassword = Convert.ToBase64String(passwordHash),
                        CreatedAt = DateTime.UtcNow,
                        MustResetPassword = true,
                        AccessFailedCount = 0,
                        IsArgonHash = true,
                        UserId = user.Id,
                        ModifiedBy = user.Id,
                        ModifiedAt = DateTime.UtcNow
                    });
                    dbContext!.SaveChanges();
                    return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, true,"Success"));
                }
                else
                {
                    transitionName = "user-reset-password-sms-fail";
                    return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false,"Sms Can not Send"));
                }
            }
            else
            {
                 transitionName = "user-reset-password-sms-fail";
                    return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false,"User Not Found"));
            }
        }
        catch (Exception ex)
        {
            transitionName = "user-reset-password-sms-fail";
            return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false,"Unexcepted Error While Sendind Sms"));
        }
        transitionName = "user-reset-password-sms-fail";
        return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false,"Unexcepted Error While Sendind Sms"));
    }
    static IResult postSmsKeySender(
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
            User? user = dbContext.Users!.FirstOrDefault(f => f.Id == recordId);
            if (user == null)
            {
                dynamic? entityData=body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData");
               string countryCode = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("phone")
               .GetProperty("countryCode").ToString();
                string prefix = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("phone")
               .GetProperty("prefix").ToString();
                string number = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("phone")
               .GetProperty("number").ToString();
                 string reference = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("reference").ToString();
               user = new User()
                {
                    Id = recordId,
                    State = "DeActive",
                    Reference =reference,
                    Phone =new Phone(){
                        CountryCode=Convert.ToInt32(countryCode),
                        Prefix=Convert.ToInt32(prefix),
                        Number=number
                    }
                };
                dbContext.Users!.Add(user);
                dbContext.SaveChanges();
            }
             Random rnd = new Random();
                int num = rnd.Next(9999);
                string smsKey = num.ToString("0000");
                bool smsSend = SendSms("Test:"+ smsKey,user,configuration,triggeredByBehalfOfAsString);
                if (smsSend)
                {

                    dbContext.UserSmsKeys!.Add(new UserSmsKey
                    {
                        Id = new Guid(),
                        SmsKey = smsKey,
                        UserId = user.Id,
                        CreatedAt = DateTime.Now,
                        CreatedBy = triggeredBy,
                        CreatedByBehalfOf=triggeredByBehalfOf
                        
                    });
                    dbContext!.SaveChanges();
                    return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, true,"Success"));
                }
                else
                {
                    transitionName = "openbanking-register-sms-fail";
                    return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false,"Sms Can not Send"));
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            transitionName = "openbanking-register-sms-fail";
            return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false,"Unexcepted Error While Sendind Sms"));
        }
        transitionName = "openbanking-register-sms-fail";
        return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredByAsString, triggeredByBehalfOfAsString, data, false,"Unexcepted Error While Sendind Sms"));
    }
   static IResult postSmsKeyControl(
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
        var triggeredBy = body.GetProperty($"TRX-{transitionName}").GetProperty("TriggeredBy").ToString();
        var triggeredByBehalfOf = body.GetProperty($"TRX-{transitionName}").GetProperty("TriggeredByBehalfOf").ToString();
         string smsKey = body.GetProperty($"TRX-{transitionName}").GetProperty("Data").GetProperty("entityData").GetProperty("smsKey").ToString();
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
        try
        {
            DateTime date=DateTime.Now.AddMinutes(-3);
            
            UserSmsKey? userSmsKey = dbContext.UserSmsKeys!.Where(f=>f.UserId==recordId&&f.CreatedAt>=date).OrderByDescending(o=>o.CreatedAt).FirstOrDefault();
            if (userSmsKey!=null&&userSmsKey.SmsKey==smsKey)
            {
               
                    return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, true,"Success"));
            }
            else
            {
                   transitionName = "openbanking-register-sms-confirm-fail";
                return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false,"Sms Key Not Found"));
            }
        }
        catch (Exception ex)
        {
            transitionName = "openbanking-register-sms-confirm-fail";
            return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false,"Unexpected error while searching sms key"));
        }
        transitionName = "openbanking-register-sms-confirm-fail";
        return Results.Ok(ZeebeMessageHelper.createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false,"Unexpected error while searching sms key"));
    }


    private static bool SendSms(string contentFromMethod, User user ,IConfiguration configuration,string triggeredBy)
    {
        amorphie.fact.core.Dtos.Phone phone = new amorphie.fact.core.Dtos.Phone();
        phone.countryCode = user!.Phone!.CountryCode;
        phone.prefix = user!.Phone!.Prefix;
        phone.number = Convert.ToInt32(user!.Phone!.Number);
        amorphie.fact.core.Dtos.SmsRequest requestSmsModel = new amorphie.fact.core.Dtos.SmsRequest()
        {
            sender = "AutoDetect",
            smsType = "Otp",
            phone = phone,
            content = contentFromMethod,
            process = new amorphie.fact.core.Dtos.Process()
            {
                action = null,
                name = "userzeebe",
                identity =triggeredBy,
                itemId = null
            },
            tags = null,
            customerNo = 0,
            citizenshipNo = ""

        };
        string bodyParams = Newtonsoft.Json.JsonConvert.SerializeObject(requestSmsModel);
        HttpClient clientHttp = new HttpClient();
        string messaginggatewayUrl=configuration["messagingGatewayUrl"]!.ToString();
        var response = clientHttp.PostAsync(messaginggatewayUrl, new StringContent(bodyParams, System.Text.Encoding.UTF8, "application/json")).Result;
        var SendSmsOtpResponse = response.Content.ReadFromJsonAsync<amorphie.fact.core.Dtos.SendSmsOtpResponse>();
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return true;
        }
        return false;
    }
}