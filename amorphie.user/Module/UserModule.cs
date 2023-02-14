using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static amorphie.user.data.User;

public static class UserModule
{

    static WebApplication _app = default!;

    public static void MapUserEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/user", getAllUser)
       .WithOpenApi()
       .WithSummary("Gets registered users.")
       .WithDescription("Returns existing users with metadata.Query parameter TcNo is can contain request or order TcNo of user.")
       .WithTags("User")
       .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

    //    _app.MapGet("/user/{countrycode}/{prefix}{number}", getPhoneUser)
    //    .WithOpenApi()
    //    .WithSummary("Gets registered users.")
    //    .WithDescription("Returns phone users records")
    //    .WithTags("User")
    //    .Produces<GetUserResponse>(StatusCodes.Status200OK)
    //    .Produces(StatusCodes.Status404NotFound);

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

        //     _app.MapPost("/user/checkUserPassword", checkUserPassword)
        //   .WithOpenApi()
        //   .WithSummary("User password check")
        //   .WithDescription("User password check.")
        //   .WithTags("User")
        //   .Produces(StatusCodes.Status200OK)
        //   .Produces(StatusCodes.Status404NotFound); 
        _app.MapGet("/user/user/{userId}/password/{password}", checkUserPassword)
           .WithOpenApi()
          .WithSummary("Check user password.")
          .WithDescription("Check user password")
          .WithTags("User")
          .Produces<GetUserResponse>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status404NotFound);



    }
    static IResult getAllUser(
      [FromServices] UserDBContext context,
      [FromQuery] string? Reference,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var query = context!.Users!
            .Skip(page * pageSize)
            .Take(pageSize);

        if (!string.IsNullOrEmpty(Reference))
        {
            query=query.Where(t => t.Reference == Reference);
        }

        var users = query.ToList();

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

    //   static IResult getPhoneUser(
    //   [FromRoute(Name = "countrycode")] Guid id,
    //    [FromRoute(Name = "id")] Guid id,
    //     [FromRoute(Name = "id")] Guid id,
    //   [FromServices] UserDBContext context,
     
    //   [FromQuery][Range(0, 100)] int page = 0,
    //   [FromQuery][Range(5, 100)] int pageSize = 100
    //   )
    // {
    //     var query = context!.Users!
    //         .Skip(page * pageSize)
    //         .Take(pageSize);

    //     if (!string.IsNullOrEmpty(Reference))
    //     {
    //         query=query.Where(t => t.Reference == Reference);
    //     }

    //     var users = query.ToList();

    //     if (users.Count() > 0)
    //     {
    //         return Results.Ok(users.Select(user =>
    //           new GetUserResponse(
    //            user.Id,
    //            user.FirstName,
    //            user.LastName,
    //            user.Reference,
    //            user.Password,
    //            user.EMail,
    //            user.Phone,
    //            user.State,
    //            user.CreatedBy,
    //            user.CreatedAt,
    //            user.ModifiedBy,
    //            user.ModifiedAt,
    //            user.CretedByBehalfOf,
    //            user.ModifiedByBehalof

    //            )
    //         ).ToArray());
    //     }
    //     else
    //         return Results.NoContent();
    // }
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
    static async Task<IResult> checkUserPassword(
        [FromRoute(Name = "userId")] Guid userId,
        [FromRoute(Name = "password")] string password,
           [FromServices] UserDBContext context
          )
    {
        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            var btyePassword = Convert.FromBase64String(user.Password);
            var salt = Convert.FromBase64String(user.Salt);
            var checkPassword = PasswordHelper.VerifyHash(password, salt, btyePassword);
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
    // static async Task<IResult> checkUserPassword(
    //        [FromBody] UserCheckPasswordRequest data,
    //         [FromServices] UserDBContext context
    //        )
    // {
    //     var user = context!.Users!.FirstOrDefault(x => x.Id == data.UserId);
    //     if (user != null)
    //     {
    //         var btyePassword = Convert.FromBase64String(user.Password);
    //         var salt = Convert.FromBase64String(user.Salt);
    //         var checkPassword = PasswordHelper.VerifyHash(data.Password, salt, btyePassword);
    //         if (checkPassword)
    //         {
    //             return Results.Created($"/user/{data.UserId}", checkPassword);
    //         }
    //         else
    //         {
    //             return Results.Problem("Passwords do not match", null);
    //         }
    //     }
    //     else
    //     {
    //         return Results.NotFound();
    //     }

    //  }

}

