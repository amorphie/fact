using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class UserTagModule
{
   
    static WebApplication _app = default!;

    public static void MapUserTagEndpoints(this WebApplication app)
    {
        _app = app;

         _app.MapGet("/usertag", getAllUserTag)
        .WithTags("UserTag")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Returns saved usertag records.";
            operation.Parameters[0].Description = "Filtering parameter. Given **usertag** is used to filter users.";
            operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
            operation.Parameters[2].Description = "Paging parameter. **Token** is returned from last query.";
            return operation;
        })
        .Produces<GetUserTagResponse[]>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

       _app.MapPost("/usertag", postUserTag)
        .WithOpenApi()
        .WithSummary("Save usertag")
        .WithDescription("")
        .WithTags("UserTag")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        

       _app.MapDelete("/usertag/{id}", deleteUserTag)
       .WithOpenApi()
       .WithSummary("Deletes usertag")
       .WithDescription("Delete usertag.")
       .WithTags("UserTag")
       .Produces<GetUserTagResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);


    }
      static IResult getAllUserTag(
        [FromServices] UserDBContext context,
        [FromQuery] string? Tag,
        [FromQuery][Range(0, 100)] int page = 0,
        [FromQuery][Range(5, 100)] int pageSize = 100
        )
    {
        var query = context!.UserTags!
            .Skip(page * pageSize)
            .Take(pageSize);

        if (!string.IsNullOrEmpty(Tag))
        {
           query= query.Where(x => x.Tag.Contains(Tag));
        }

        var userTags = query.ToList();

        if (userTags.Count() > 0)
        {
            return Results.Ok(userTags.Select(userTag =>
              new GetUserTagResponse(
               userTag.Id,
               userTag.Tag,
               userTag.UserId,
               userTag.CreatedBy,
               userTag.CreatedAt,
               userTag.ModifiedBy,
               userTag.ModifiedAt,
               userTag.CretedByBehalfOf,
               userTag.ModifiedByBehalof
               
                )
            ).ToArray());
        }
        else
            return Results.NoContent();
    }
 static async Task<IResult> postUserTag(
        [FromBody] PostUserTagRequest data,
        [FromServices] UserDBContext context
        )
    {
       
        var userTag = context!.UserTags!
          .FirstOrDefault(x => x.Tag==data.Tag && x.UserId==data.UserId );


        if (userTag== null)
        {
            var newRecord = new UserTag { 
             Id = Guid.NewGuid(),
             Tag = data.Tag,
             UserId=data.UserId,
             CreatedAt = DateTime.Now,
             CreatedBy = data.CreatedBy,
             CretedByBehalfOf = data.CretedByBehalfOf
             };
            context!.UserTags!.Add(newRecord);
            context.SaveChanges();
            return Results.Created($"/usertag/{data.Tag}", newRecord);
        }
        else{
              var hasChanges = false;
            // Apply update to only changed fields.
            if (data.Tag != null && data.Tag != userTag.Tag) {  userTag.Tag=data.Tag ; hasChanges = true; }
            if (data.UserId != null && data.UserId != userTag.UserId) { userTag.UserId = data.UserId; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != userTag.ModifiedBy) { userTag.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != userTag.ModifiedByBehalof) { userTag.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
                userTag.ModifiedAt=DateTime.Now;
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
     static IResult deleteUserTag(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.UserTags?.FirstOrDefault(t => t.Id == id);

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

