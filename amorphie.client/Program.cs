using amorphie.fact.data;
using Microsoft.EntityFrameworkCore;
using amorphie.core.Identity;
using amorphie.core.Extension;
using FluentValidation;
using System.Reflection;
using amorphie.core.Swagger;
using Elastic.Apm.NetCoreAll;
using Dapr.Client;

ThreadPool.SetMinThreads(50, 50);

var builder = WebApplication.CreateBuilder(args);
var client = new DaprClientBuilder().Build();
using (var tokenSource = new CancellationTokenSource(20000))
{
    try
    {
        await client.WaitForSidecarAsync(tokenSource.Token);
    }
    catch (System.Exception ex)
    {
        Console.WriteLine("Dapr Sidecar Doesn't Respond");
        return;
    }
}

await builder.Configuration.AddVaultSecrets("user-secretstore", new string[] { "user-secretstore" });
var postgreSql = builder.Configuration["postgresql"];
await builder.SetSecrets();

// var postgreSql = "Host=localhost:5432;Database=users;Username=postgres;Password=postgres";
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();


builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => { options.OperationFilter<AddSwaggerParameterFilter>(); });

builder.Services.AddScoped<IBBTIdentity, FakeIdentity>();

var assemblies = new Assembly[]
{
    typeof(ClientValidator).Assembly, typeof(ClientMapper).Assembly
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
app.MapHealthChecks("/health");
app.UseCloudEvents();
app.UseRouting();
app.MapSubscribeHandler();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

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
