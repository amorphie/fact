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
    }
    static IResult postSmsSender(
          [FromBody] dynamic body,
         [FromServices] UserDBContext dbContext,
          HttpRequest request,
          HttpContext httpContext,
          [FromServices] DaprClient client
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
                string Password = Guid.NewGuid().ToString().Substring(0, 6);
                bool smsSend = SendSms(Password, user);
                if (smsSend)
                {
                    var salt = Convert.FromBase64String(user.Salt);
                    var password = ArgonPasswordHelper.HashPassword(Password, salt);

                    dbContext.UserPasswords!.Add(new UserPassword
                    {
                        Id = new Guid(),
                        HashedPassword = Convert.ToBase64String(password),
                        CreatedAt = DateTime.UtcNow,
                        MustResetPassword = true,
                        AccessFailedCount = 0,
                        IsArgonHash = true,
                        UserId = user.Id,
                        ModifiedBy = user.Id,
                        ModifiedAt = DateTime.UtcNow
                    });
                    dbContext!.SaveChanges();
                    return Results.Ok(createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, true));
                }
                else
                {
                    transitionName = "user-reset-password-sms-fail";
                    return Results.Ok(createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false));
                }
            }
            else
            {
                return Results.BadRequest("User Not Found");
            }
        }
        catch (Exception ex)
        {
            transitionName = "user-reset-password-sms-fail";
            return Results.Ok(createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false));
        }
        transitionName = "user-reset-password-sms-fail";
        return Results.Ok(createMessageVariables(body, transitionName, triggeredBy, triggeredByBehalfOf, data, false));
    }
    private static dynamic createMessageVariables(dynamic body, string _transitionName, string TriggeredBy, string TriggeredByBehalfOf, dynamic _data, bool success)
    {
        dynamic variables = new Dictionary<string, dynamic>();

        variables.Add("EntityName", body.GetProperty("EntityName").ToString());
        variables.Add("RecordId", body.GetProperty("RecordId").ToString());
        variables.Add("InstanceId", body.GetProperty("InstanceId").ToString());
        variables.Add("LastTransition", _transitionName);
        if (success)
            variables.Add("Status", "OK");
        else
        {
            variables.Add("Status", "NOTOK");
        }
        dynamic targetObject = new System.Dynamic.ExpandoObject();
        targetObject.Data = _data;
        targetObject.TriggeredBy = TriggeredBy;
        targetObject.TriggeredByBehalfOf = TriggeredByBehalfOf;


        variables.Add($"TRX-{_transitionName}", targetObject);
        return variables;
    }
    private static bool SendSms(string Password, User user)
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
            content = "Yeni şifreniz:" + Password ,
            process = new amorphie.fact.core.Dtos.Process()
            {
                action = null,
                name = "userzeebe",
                identity = "u05645",
                itemId = null
            },
            tags = null,
            customerNo = 0,
            citizenshipNo = ""

        };
        string bodyParams = Newtonsoft.Json.JsonConvert.SerializeObject(requestSmsModel);
        HttpClient clientHttp = new HttpClient();

        var response = clientHttp.PostAsync("https://test-messaginggateway.burgan.com.tr/api/v2/Messaging/sms/message", new StringContent(bodyParams, System.Text.Encoding.UTF8, "application/json")).Result;
        var SendSmsOtpResponse = response.Content.ReadFromJsonAsync<amorphie.fact.core.Dtos.SendSmsOtpResponse>();
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return true;
        }
        return false;
    }
}