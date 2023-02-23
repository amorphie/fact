using amorphie.user.data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SecretExtensions;

var builder = WebApplication.CreateBuilder(args);
await builder.Configuration.AddVaultSecrets("user-secretstore", "user-secretstore");
var postgreSql = builder.Configuration["postgresql"];
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();


builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDBContext>
    (options => options.UseNpgsql(postgreSql,b => b.MigrationsAssembly("amorphie.user.data")));

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
