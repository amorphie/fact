using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class SecurityImageModule
{

    static WebApplication _app = default!;

    public static void MapSecurityImageEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/securityimage", getAllSecurityImage)
       .WithTags("SecurityImage")
       .WithOpenApi(operation =>
       {
           operation.Summary = "Returns saved securityimage records.";
           operation.Parameters[0].Description = "Filtering parameter. Given **securityimage** is used to filter securityimages.";
           operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
           return operation;
       })
       .Produces<GetSecurityImageResponse[]>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status204NoContent);

        _app.MapPost("/securityimage", postSecurityImage)
         .WithOpenApi()
         .WithSummary("Save securityimage")
         .WithDescription("")
         .WithTags("SecurityImage")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/securityimage/{id}", deleteSecurityImage)
        .WithOpenApi()
        .WithSummary("Deletes securityimage")
        .WithDescription("Delete securityimage.")
        .WithTags("SecurityImage")
        .Produces<GetSecurityImageResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
    static IResult getAllSecurityImage(
      [FromServices] UserDBContext context,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var query = context!.SecurityImages!
            .Skip(page * pageSize)
            .Take(pageSize);

        var securityImages = query.ToList();

        if (securityImages.Count() > 0)
        {
            return Results.Ok(securityImages.Select(securityImages =>
              new GetSecurityImageResponse(
               securityImages.Id,
               securityImages.Image,
               securityImages.CreatedBy,
               securityImages.CreatedAt,
               securityImages.ModifiedBy,
               securityImages.ModifiedAt,
               securityImages.CretedByBehalfOf,
               securityImages.ModifiedByBehalof
                )
            ).ToArray());
        }
        else
            return Results.NoContent();
    }
    static async Task<IResult> postSecurityImage(
           [FromBody] PostSecurityImageRequest data,
           [FromServices] UserDBContext context
           )
    {

        var securityImage = context!.SecurityImages!
          .FirstOrDefault(x => x.Image == data.Image);


        if (securityImage == null)
        {
            var newRecord = new SecurityImage
            {
                Id = Guid.NewGuid(),
                Image = data.Image,
                CreatedAt = DateTime.Now,
                CreatedBy = data.CreatedBy,
                CretedByBehalfOf = data.CretedByBehalfOf

            };
            context!.SecurityImages!.Add(newRecord);
            context.SaveChanges();
            return Results.Created($"/securityimage/{data.Image}", newRecord);
        }
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.Image != null && data.Image != securityImage.Image) { securityImage.Image = data.Image; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != securityImage.ModifiedBy) { securityImage.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != securityImage.ModifiedByBehalof) { securityImage.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
                securityImage.ModifiedAt=DateTime.Now;
            
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
        return Results.Conflict("Request is already used for another record.");
    }
    static IResult deleteSecurityImage(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.SecurityImages?.FirstOrDefault(t => t.Id == id);

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

