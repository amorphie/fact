using Microsoft.EntityFrameworkCore;
using amorphie.core.security.Extensions;
using amorphie.fact.data;
using amorphie.core.Identity;
using amorphie.core.Repository;
using System.Reflection;
using FluentValidation;
using amorphie.core.Extension;

var builder = WebApplication.CreateBuilder(args);
await builder.Configuration.AddVaultSecrets("user-secretstore",new string[]{"user-secretstore"});
var postgreSql = builder.Configuration["postgresql"];
// var postgreSql = "Host=localhost:5432;Database=users;Username=postgres;Password=postgres";
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBBTIdentity, FakeIdentity>();
builder.Services.AddScoped(typeof(IBBTRepository<,>), typeof(BBTRepository<,>));

var assemblies = new Assembly[]
                {
                     typeof(UserValidator).Assembly, typeof(UserMapper).Assembly
                };

builder.Services.AddValidatorsFromAssemblies(assemblies);
builder.Services.AddAutoMapper(assemblies);

builder.Services.AddDbContext<UserDBContext>
    (options => options.UseNpgsql(postgreSql, b => b.MigrationsAssembly("amorphie.fact.data")));

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<UserDBContext>();
db.Database.Migrate();

app.UseCloudEvents();
app.UseRouting();
app.MapSubscribeHandler();

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