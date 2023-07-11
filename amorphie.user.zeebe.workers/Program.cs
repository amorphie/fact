using Microsoft.EntityFrameworkCore;
using amorphie.core.security.Extensions;
using amorphie.fact.data;
using amorphie.core.Identity;
using System.Reflection;
using FluentValidation;
using amorphie.core.Extension;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);
var daprClient = new DaprClientBuilder().Build();
await builder.Configuration.AddVaultSecrets("user-secretstore", new string[] { "user-secretstore" });
var postgreSql = builder.Configuration["postgresql"];

 //var postgreSql = "Host=localhost:5432;Database=users;Username=postgres;Password=postgres";
// Add services to the container.
builder.Services.AddDaprClient();
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDBContext>
    (options => options.UseNpgsql(postgreSql, b => b.MigrationsAssembly("amorphie.fact.data")));

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<UserDBContext>();
// db.Database.Migrate();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.UseRouting();
app.MapSubscribeHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapZeebeSmsSenderEndpoints();

app.Run();
