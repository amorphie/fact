using amorphie.user.data;
using Microsoft.EntityFrameworkCore;
namespace amorphie.user;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddJsonConsole();

        builder.Services.AddDaprClient();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<UserDBContext>
        (options => options.UseNpgsql("Host=localhost:5432;Database=users;Username=postgres;Password=postgres"));
        var app = builder.Build();

        app.UseCloudEvents();
        app.UseRouting();
        app.MapSubscribeHandler();

        app.UseSwagger();
        app.UseSwaggerUI();

         app.MapUserEndpoints();
         app.MapUserTagEndpoints();
         app.MapUserSecurityQuestionEndpoints();

        // app.MapGet("/", () => "Hello World!");
        // app.MapGet("/user", ([FromQuery] string? TcNo) => { })
        //     .WithOpenApi(operation =>
        //         {
        //             operation.Summary = "Returns queried users.";
        //             operation.Parameters[0].Description = "Full or partial name of TcNo to be queried.";
        //             return operation;
        //         })
        //      .Produces<GetUserResponse[]>(StatusCodes.Status200OK)
        //     .Produces(StatusCodes.Status204NoContent)
        // ;
        try
        {
            app.Logger.LogInformation("Starting application...");
            app.Run();
        }
        catch (Exception ex)
        {
            app.Logger.LogCritical(ex, "Aplication is terminated unexpectedly ");
        }
        //
    }
}
