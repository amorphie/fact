using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class UserSecurityImageModule
{

    static WebApplication _app = default!;

    public static void MapUserSecurityImageEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/userSecurityImage", getAllUserSecurityImage)
        .WithOpenApi()
       .WithSummary("Returns saved usersecurityimage records.")
       .WithDescription("Returns existing usersecurityimage with metadata.Query parameter usersecurityimage is can contain request or order SecurityQuestion of usersecurityimages.")
       .WithTags("UserSecurityImage")
       .Produces<GetUserSecurityImageResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);


        _app.MapPost("/userSecurityImage", postUserSecurityImage)
         .WithOpenApi()
         .WithSummary("Save usersecurityimage.")
         .WithDescription("Save usersecurityimage.")
         .WithTags("UserSecurityImage")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/userSecurityImage/{id}", deleteUserSecurityImage)
        .WithOpenApi()
        .WithSummary("Deletes usersecurityimage")
        .WithDescription("Delete usersecurityimage.")
        .WithTags("UserSecurityImage")
        .Produces<GetUserSecurityImageResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/userSecurityImage/user/{userId}/image/{image}", userCheckImage)
        .WithOpenApi()
       .WithSummary("Check image.")
       .WithDescription("Check image")
       .WithTags("UserSecurityImage")
       .Produces<GetUserSecurityImageResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);


    }
    static IResult getAllUserSecurityImage(
      [FromServices] UserDBContext context,
      [FromQuery] Guid UserId,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var query = context!.UserSecurityImages!

            .Skip(page * pageSize)
            .Take(pageSize);

        if (UserId != null)
        {
            query = query.Where(t => t.UserId == UserId);
        }

        var userSecurityImages = query.ToList();

        if (userSecurityImages.Count() > 0)
        {
            return Results.Ok(userSecurityImages.Select(userSecurityImage =>
              new GetUserSecurityImageResponse(
               userSecurityImage.Id,
               userSecurityImage.SecurityImage,
               userSecurityImage.UserId,
               userSecurityImage.CreatedBy,
               userSecurityImage.CreatedAt,
               userSecurityImage.ModifiedBy,
               userSecurityImage.ModifiedAt,
               userSecurityImage.CretedByBehalfOf,
               userSecurityImage.ModifiedByBehalof

                )
            ).ToArray());
        }
        else
            return Results.NoContent();
    }
    static async Task<IResult> postUserSecurityImage(
           [FromBody] PostUserSecurityImageRequest data,
           [FromServices] UserDBContext context
           )
    {

        var userSecurityImage = context!.UserSecurityImages!
          .FirstOrDefault(x => x.UserId == data.UserId);
        var user = context!.Users.FirstOrDefault(x => x.Id == data.UserId);

        if (userSecurityImage == null)
        {
            if (user.Salt != null)
            {
                var salt = Convert.FromBase64String(user.Salt);

                var password = PasswordHelper.HashPassword(data.SecurityImage, salt);
                var result = Convert.ToBase64String(password);


                var newRecord = new UserSecurityImage
                {
                    Id = Guid.NewGuid(),
                    SecurityImage = result,
                    UserId = data.UserId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = data.CreatedBy,
                    CretedByBehalfOf = data.CretedByBehalfOf
                };
                context!.UserSecurityImages!.Add(newRecord);
                context.SaveChanges();
                return Results.Created($"/userSecurityImage/{data.UserId}", newRecord);
            }
            else
            {
                return Results.NotFound("User salt not found");
            }
        }
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.SecurityImage != null)
            {
                if (user.Salt != null)
                {
                    var bytePassword = Convert.FromBase64String(userSecurityImage.SecurityImage);
                    var salt = Convert.FromBase64String(user.Salt);
                    var checkPassword = PasswordHelper.VerifyHash(data.SecurityImage, salt, bytePassword);
                    if (!checkPassword)
                    {
                        var password = PasswordHelper.HashPassword(data.SecurityImage, salt);
                        user.Password = Convert.ToBase64String(password);
                        hasChanges = true;
                    }
                    if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != userSecurityImage.ModifiedByBehalof) { userSecurityImage.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
                    if (data.ModifiedBy != null && data.ModifiedBy != userSecurityImage.ModifiedBy) { userSecurityImage.ModifiedBy = data.ModifiedBy; hasChanges = true; }
                    userSecurityImage.ModifiedAt = DateTime.Now;

                }

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
            else
            {
                return Results.NotFound("User salt not found");
            }

        }
        return Results.Conflict("Request  is already used for another record.");
    }
    static IResult deleteUserSecurityImage(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.UserSecurityImages?.FirstOrDefault(t => t.Id == id);

        if (recordToDelete == null)
        {
            return Results.NotFound();
        }
        else
        {
            context!.Remove(recordToDelete);
            context.SaveChanges();
            return Results.Ok();
        }
    }
    static async Task<IResult> userCheckImage(
        [FromRoute(Name = "userId")] Guid userId,
        [FromRoute(Name = "image")] string image,
           //    [FromQuery] Guid UserId,
           //     [FromQuery] string Image,
           [FromServices] UserDBContext context
          )
    {
        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            var securityImage = context!.UserSecurityImages!.FirstOrDefault(x => x.UserId == userId);
            if (securityImage != null)
            {
                var byteImage = Convert.FromBase64String(securityImage.SecurityImage);
                var salt = Convert.FromBase64String(user.Salt);
                var checkPassword = PasswordHelper.VerifyHash(image, salt, byteImage);
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
                return Results.NotFound("SecurityImage definition is not found. Please check userId is exists in definitions.");
            }
        }
        else
        {
            return Results.NotFound("User is not found.");
        }

    }
}

