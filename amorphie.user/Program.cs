using Microsoft.EntityFrameworkCore;
using amorphie.fact.data;
using amorphie.core.Identity;
using System.Reflection;
using FluentValidation;
using amorphie.core.Extension;
using amorphie.core.Middleware.Logging;
using Dapr.Client;
using Elastic.Apm.NetCoreAll;
using Microsoft.EntityFrameworkCore.Infrastructure;

ThreadPool.SetMinThreads(20, 20);
    
var builder = WebApplication.CreateBuilder(args);
var client = new DaprClientBuilder().Build();
using (var tokenSource = new CancellationTokenSource(20000))
{
    try
    {
        await client.WaitForSidecarAsync(tokenSource.Token);
    }
    catch (System.Exception)
    {
        Console.WriteLine("Dapr Sidecar Doesn't Respond");
        return;
    }

}
await builder.Configuration.AddVaultSecrets("user-secretstore", new string[] { "user-secretstore" });
var postgreSql = builder.Configuration["postgresql"];
// var postgreSql = "Host=localhost:5432;Database=users;Username=postgres;Password=postgres";

builder.Services.AddDaprClient();
builder.AddSeriLog<AmorphieLogEnricher>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBBTIdentity, FakeIdentity>();

var assemblies = new Assembly[]
                {
                     typeof(UserValidator).Assembly, typeof(UserMapper).Assembly
                };

builder.Services.AddValidatorsFromAssemblies(assemblies);
builder.Services.AddAutoMapper(assemblies);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddDbContext<UserDBContext>
    (options => options.UseNpgsql(postgreSql, b => b.MigrationsAssembly("amorphie.fact.data")));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
})
.AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
;

builder.Services.AddHealthChecks();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseAllElasticApm(app.Configuration);
}

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<UserDBContext>();
db.Database.Migrate();


app.MapPost("/public/device/save/{clientCode}", UserDevicePublic.saveDevice)
            .Produces(StatusCodes.Status200OK);
app.MapGet("/public/device/{clientCode}/{reference}", UserDevicePublic.GetActiveDevice)
            .Produces(StatusCodes.Status200OK);
app.MapPut("/public/device/remove/{clientCode}/{reference}/", UserDevicePublic.removeDevice)
.Produces(StatusCodes.Status200OK);

app.MapHealthChecks("/health");

app.UseCloudEvents();
app.UseRouting();
app.MapSubscribeHandler();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.UseLoggingHandlerMiddlewares();
try
{
    app.Logger.LogInformation("Starting application...");
    app.AddRoutes();
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Aplication is terminated unexpectedly ");
}
