using System;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecretExtensions;

var builder = WebApplication.CreateBuilder(args);
await builder.Configuration.AddVaultSecrets("user-secretstore", "user-secretstore");
var postgresql = builder.Configuration["postgresql"];
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDBContext>
(options => options.UseNpgsql(postgresql));
var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.UseCloudEvents();
app.UseRouting();
app.MapSubscribeHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.MapUserEndpoints();
app.MapUserTagEndpoints();
app.MapSecurityQuestionEndpoints();
app.MapUserSecurityQuestionEndpoints();
app.MapSecurityImageEndpoints();
app.MapUserSecurityImageEndpoints();
app.MapUserDeviceEndpoints();

try
{
    app.Logger.LogInformation("Starting application...");
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Aplication is terminated unexpectedly ");
}
