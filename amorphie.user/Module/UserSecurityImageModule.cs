using System.ComponentModel.DataAnnotations;
using amorphie.user.data;
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

        if (UserId!=null)
        {
            query.Where(t => t.UserId==UserId);
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
        [FromBody] PostSecurityImageRequest data,
        [FromServices] UserDBContext context
        )
    {

        var userSecurityImage = context!.UserSecurityImages!
          .FirstOrDefault(x => x.UserId==data.UserId );
        var user=context!.Users.FirstOrDefault(x=>x.Id==data.UserId);

        if (userSecurityImage== null)
        {
           
            var salt = Convert.FromBase64String(user.Salt);

            var password = PasswordHelper.HashPassword(data.Image, salt);
            var result = Convert.ToBase64String(password);
   
            var newRecord = new UserSecurityImage {
                 Id = Guid.NewGuid(), 
                 SecurityImage =result,
                 UserId=data.UserId,
                 CreatedAt = DateTime.Now,
                 CreatedBy = data.CreatedBy,
                 CretedByBehalfOf = data.CretedByBehalfOf
                  };
            context!.UserSecurityImages!.Add(newRecord);
            context.SaveChanges();
            return Results.Created($"/userSecurityImage/{data.UserId}", newRecord);
        }
        else{
              var hasChanges = false;
            // Apply update to only changed fields.
            if (data.Image != null)
            {
                var bytePassword = Convert.FromBase64String(userSecurityImage.SecurityImage);
                var salt = Convert.FromBase64String(user.Salt);
                var checkPassword = PasswordHelper.VerifyHash(data.Image, salt, bytePassword);
                if (!checkPassword)
                {
                    var password = PasswordHelper.HashPassword(data.Image, salt);
                    user.Password = Convert.ToBase64String(password);
                    hasChanges=true;
                }
                 if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != userSecurityImage.ModifiedByBehalof) { userSecurityImage.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
                 if (data.ModifiedBy != null && data.ModifiedBy != userSecurityImage.ModifiedBy) { userSecurityImage.ModifiedBy = data.ModifiedBy; hasChanges = true; }
                     userSecurityImage.ModifiedAt=DateTime.Now;

            }
         
            if (hasChanges)
            {
                context!.SaveChanges();
                return Results.Ok(userSecurityImage);
            }
            else
            {
                return Results.Problem("Not Modified.", null, 304);
            }
        
        }
        return Results.Conflict("Request or Order template is already used for another record.");
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
    }

