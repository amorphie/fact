using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapr.Client;
using static amorphie.user.data.User;
using Microsoft.EntityFrameworkCore;

public static class UserModule
{
    const string STATE_STORE = "amorphie-cache";
    static WebApplication _app = default!;

    public static void MapUserEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/user", getAllUsers)
       .WithOpenApi()
       .WithSummary("Gets registered users.")
       .WithDescription("Returns existing users with metadata.Query parameter reference is can contain request or order reference of user.")
       .WithTags("User")
       .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/user/phone/countrycode/{countrycode}/prefix/{prefix}/number/{number}", getPhoneUser)
        .WithOpenApi()
        .WithSummary("Returns phone users records.")
        .WithDescription("Returns phone users records")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/user/email/{email}/", getEmailUser)
      .WithOpenApi()
      .WithSummary("Returns email users records.")
      .WithDescription("Returns email users records")
      .WithTags("User")
      .Produces<GetUserResponse>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

        _app.MapPost("/user", postUser)
        .WithOpenApi()
        .WithSummary("Save user")
        .WithDescription("It is update or creates new user.")
        .WithTags("User")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/user/{id}", deleteUser)
        .WithOpenApi()
        .WithSummary("Deletes user")
        .WithDescription("Delete user.")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/user/userId/{userId}/password/{password}", checkUserPassword)
        .WithOpenApi()
        .WithSummary("Check user password.")
        .WithDescription("Check user password")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapPost("/user/{userId}/updatePassword", updateUserPassword)
        .WithOpenApi()
        .WithSummary("Update user password.")
        .WithDescription("Update user password.")
        .WithTags("User")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        _app.MapPost("/user/{userId}/updateEmail", updateUserEmail)
       .WithOpenApi()
       .WithSummary("Update user mail.")
       .WithDescription("Update user mail.")
       .WithTags("User")
       .Produces(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status201Created)
       .Produces(StatusCodes.Status409Conflict);

        _app.MapPost("/user/{userId}/updatePhone", updateUserPhone)
      .WithOpenApi()
      .WithSummary("Update user phone.")
      .WithDescription("Update user phone.")
      .WithTags("User")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status201Created)
      .Produces(StatusCodes.Status409Conflict);

    }
    async  static Task<IResult> getAllUsers(
      [FromServices] UserDBContext context,
      [FromQuery] string? Reference,
      HttpContext httpContext,
      [FromServices] DaprClient client,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var cacheData = await client.GetStateAsync<GetUserResponse[]>(STATE_STORE, "GetAllUsers");
        if (cacheData != null)
        {
            httpContext.Response.Headers.Add("X-Cache", "Hit");
            return Results.Ok(cacheData);
        }
        var query = context!.Users!
            .Include(d =>d.UserTags)
            .Skip(page * pageSize)
            .Take(pageSize);

        if (!string.IsNullOrEmpty(Reference))
        {
            query = query.Where(t => t.Reference == Reference);
        }

        var users = query.ToList();

        if (users.Count() > 0)
        {
            var response = users.Select(user =>
               new GetUserResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Reference,
                user.Password,
                user.EMail,
                user.Phone,
                user.State,
                user.UserTags.Select(x=>x.Tag).ToArray(),
                //user.UserTags.ToList(),
                user.CreatedBy,
                user.CreatedAt,
                user.ModifiedBy,
                user.ModifiedAt,
                user.CretedByBehalfOf,
                user.ModifiedByBehalof

                )
             ).ToArray();
            var metadata = new Dictionary<string, string> { { "ttlInSeconds", "15" } };
             await client.SaveStateAsync(STATE_STORE, "GetAllUsers", response, metadata: metadata);
            httpContext.Response.Headers.Add("X-Cache", "Miss");
            return Results.Ok(response);
        }
        else
            return Results.NoContent();
    }

    static IResult getPhoneUser(
    [FromRoute(Name = "countrycode")] int countryCode,
    [FromRoute(Name = "prefix")] int prefix,
    [FromRoute(Name = "number")] int number,
    [FromServices] UserDBContext context,
    [FromQuery][Range(0, 100)] int page = 0,
    [FromQuery][Range(5, 100)] int pageSize = 100
    )
    {
        var query = context!.Users!
            .Include(d =>d.UserTags)
            .Skip(page * pageSize)
            .Take(pageSize);

        var users = query.Where(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number).ToList();

        if (users.Count() > 0)
        {
            return Results.Ok(users.Select(user =>
              new GetUserResponse(
               user.Id,
               user.FirstName,
               user.LastName,
               user.Reference,
               user.Password,
               user.EMail,
               user.Phone,
               user.State,
               user.UserTags.Select(x=>x.Tag).ToArray(),
               user.CreatedBy,
               user.CreatedAt,
               user.ModifiedBy,
               user.ModifiedAt,
               user.CretedByBehalfOf,
               user.ModifiedByBehalof

               )
            ).ToArray());
        }
        else
            return Results.NoContent();
    }
    static IResult getEmailUser(
   [FromRoute(Name = "email")] string email,
   [FromServices] UserDBContext context,
   [FromQuery][Range(0, 100)] int page = 0,
   [FromQuery][Range(5, 100)] int pageSize = 100
   )
    {
        var query = context!.Users!
            .Include(d =>d.UserTags)
            .Skip(page * pageSize)
            .Take(pageSize);
        if (!String.IsNullOrEmpty(email))
        {
            var users = query.Where(t => t.EMail == email).ToList();

            if (users.Count() > 0)
            {
                return Results.Ok(users.Select(user =>
                  new GetUserResponse(
                   user.Id,
                   user.FirstName,
                   user.LastName,
                   user.Reference,
                   user.Password,
                   user.EMail,
                   user.Phone,
                   user.State,
                  user.UserTags.Select(x=>x.Tag).ToArray(),
                   user.CreatedBy,
                   user.CreatedAt,
                   user.ModifiedBy,
                   user.ModifiedAt,
                   user.CretedByBehalfOf,
                   user.ModifiedByBehalof


                   )
                ).ToArray());
            }
            else
            {
                return Results.NoContent();
            }
        }
        else
        {
            return Results.NotFound("Email is null");
        }
    }
    static async Task<IResult> postUser(
           [FromBody] PostUserRequest data,
            [FromServices] UserDBContext context
           )

    {
        // Check any Reference,Name,Surname is exists ?
        var user = context!.Users!
          .FirstOrDefault(x => x.Reference == data.Reference && x.FirstName == data.FirstName && x.LastName == data.LastName);


        if (user == null)
        {
            var salt = PasswordHelper.CreateSalt();
            var password = PasswordHelper.HashPassword(data.Password, salt);
            var result = Convert.ToBase64String(password);
            var newRecord = new User
            {
                Id = Guid.NewGuid(),
                FirstName = data.FirstName,
                LastName = data.LastName,
                Password = result,
                Reference = data.Reference,
                State = data.State,
                CreatedAt = DateTime.Now,
                CreatedBy = data.CreatedBy,
                CretedByBehalfOf = data.CretedByBehalfOf,
                EMail = data.EMail,
                Phone = data.Phone,
                Salt = Convert.ToBase64String(salt)
            };
            context!.Users!.Add(newRecord);
            context.SaveChanges();
            return Results.Created($"/user/{data.Reference}", newRecord);
        }
        else
        {

            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.FirstName != null && data.FirstName != user.FirstName) { user.FirstName = data.FirstName; hasChanges = true; }
            if (data.LastName != null && data.LastName != user.LastName) { user.LastName = data.LastName; hasChanges = true; }
            if (data.Password != null)
            {
                var btyePassword = Convert.FromBase64String(user.Password);
                var salt = Convert.FromBase64String(user.Salt);
                var checkPassword = PasswordHelper.VerifyHash(data.Password, salt, btyePassword);
                if (!checkPassword)
                {
                    var password = PasswordHelper.HashPassword(data.Password, salt);
                    user.Password = Convert.ToBase64String(password);
                    hasChanges = true;
                }

            }

            if (data.Reference != null && data.Reference != user.Reference) { user.Reference = data.Reference; hasChanges = true; }
            if (data.State != null && data.State != user.State) { user.State = data.State; hasChanges = true; }
            if (data.EMail != null && data.EMail != user.EMail) { user.EMail = data.EMail; hasChanges = true; }
            if (data.Phone != null && data.Phone != user.Phone) { user.Phone = data.Phone; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != user.ModifiedByBehalof) { user.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != user.ModifiedBy) { user.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            user.ModifiedAt = DateTime.Now;
            if (hasChanges)
            {
                context!.SaveChanges();
                return Results.Ok(data);
            }
            else
            {
                return Results.Problem("Not Modified.", null, 304);
            }
        }

        return Results.Conflict("Request  is already used for another record.");
    }

    static IResult deleteUser(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var existingRecord = context?.Users?.FirstOrDefault(t => t.Id == id);

        if (existingRecord == null)
        {
            return Results.NotFound();
        }
        else
        {
            context!.Remove(existingRecord);
            context.SaveChanges();
            return Results.Ok();
        }
    }
    static async Task<IResult> updateUserPassword(
        [FromRoute(Name = "userId")] Guid userId,
        [FromBody] UserPasswordUpdateRequest request,
        [FromServices] UserDBContext context
          )
    {
        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {

                var bytePassword = Convert.FromBase64String(user.Password);
                var salt = Convert.FromBase64String(user.Salt);
                var checkPassword = PasswordHelper.VerifyHash(request.oldPassword, salt, bytePassword);
                if (checkPassword)
                {

                    var password = PasswordHelper.HashPassword(request.newPassord, salt);
                    user.Password = Convert.ToBase64String(password);

                    context!.SaveChanges();
                    return Results.Ok(user);
                }
                else
                {
                    return Results.Problem("Old Passwords do not match", null);

                }

        }
        else
        {
            return Results.NotFound("User is not found");
        }
    }
    static async Task<IResult> checkUserPassword(
    [FromRoute(Name = "userId")] Guid userId,
    [FromRoute(Name = "password")] string password,
    [FromServices] UserDBContext context
    )
    {
        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            var bytePassword = Convert.FromBase64String(user.Password);
            var salt = Convert.FromBase64String(user.Salt);
            var checkPassword = PasswordHelper.VerifyHash(password, salt, bytePassword);
            if (checkPassword)
            {
                return Results.Created($"/user/{userId}", checkPassword);
            }
            else
            {
                return Results.Problem("Passwords do not match", null);
            }
        }
        else
        {
            return Results.NotFound("User is not found");
        }
    }
    static async Task<IResult> updateUserEmail(
          [FromRoute(Name = "userId")] Guid userId,
          [FromQuery] string newEmail,
          [FromServices] UserDBContext context
            )
    {

        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {

                user.EMail = newEmail;
                context!.SaveChanges();
                return Results.Ok(user);

        }
        else
        {
            return Results.NotFound("User is not found");
        }
    }
    static async Task<IResult> updateUserPhone(
              [FromRoute(Name = "userId")] Guid userId,
              [FromQuery] int countryCode,
              [FromQuery] int prefix,
              [FromQuery] int number,
              [FromServices] UserDBContext context
     )
    {

        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {

            user.Phone.CountryCode = countryCode;
            user.Phone.Prefix = prefix;
            user.Phone.Number = number;
            context!.SaveChanges();
            return Results.Ok(user);
        }
        else
        {
            return Results.NotFound("User is not found");
        }
    }

}
