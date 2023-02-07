using System.ComponentModel.DataAnnotations;
using amorphie.user.data;
using Microsoft.AspNetCore.Mvc;

public static class SecurityQuestionModule
{
   
    static WebApplication _app = default!;

    public static void MapSecurityQuestionEndpoints(this WebApplication app)
    {
        _app = app;

         _app.MapGet("/securityQuestion", getAllSecurityQuestion)
        .WithTags("SecurityQuestion")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Returns saved usertag records.";
            operation.Parameters[0].Description = "Filtering parameter. Given **securityQuestion** is used to filter securityQuestions.";
            operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
            return operation;
        })
        .Produces<GetUserResponse[]>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent);

       _app.MapPost("/securityQuestion", postSecurityQuestion)
        .WithOpenApi()
        .WithSummary("Save securityquestion")
        .WithTags("SecurityQuestion")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

       _app.MapDelete("/securityQuestion/{id}", deleteSecurityQuestion)
       .WithOpenApi()
       .WithSummary("Deletes securityquestion")
       .WithDescription("Delete securityquestion.")
       .WithTags("SecurityQuestion")
        .Produces<GetUserDeviceResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

       

    }
      static IResult getAllSecurityQuestion(
        [FromServices] UserDBContext context,
        [FromQuery] string? Question,
        [FromQuery][Range(0, 100)] int page = 0,
        [FromQuery][Range(5, 100)] int pageSize = 100
        )
    {
        var query = context!.SecurityQuestions!
            .Skip(page * pageSize)
            .Take(pageSize);

        if (!string.IsNullOrEmpty(Question))
        {
            query.Where(t => t.Question==Question);
        }

        var securityQuesitons = query.ToList();

        if (securityQuesitons.Count() > 0)
        {
            return Results.Ok(securityQuesitons.Select(securityQuestion =>
              new GetSecurityQuestionResponse(
               securityQuestion.Id,
               securityQuestion.Question,
               securityQuestion.CreatedBy,
               securityQuestion.CreatedAt,
               securityQuestion.ModifiedBy,
               securityQuestion.ModifiedAt,
               securityQuestion.CretedByBehalfOf,
               securityQuestion.ModifiedByBehalof
                )
            ).ToArray());
        }
        else
            return Results.NoContent();
    }
 static async Task<IResult> postSecurityQuestion(
        [FromBody] PostSecurityQuestionRequest data,
        [FromServices] UserDBContext context
        )
    {
        var securityQuestion = context!.SecurityQuestions!
          .FirstOrDefault(x => x.Question==data.Question);
        if (securityQuestion== null)
        {
            var newRecord = new SecurityQuestion { 
              Id = Guid.NewGuid(),
              Question = data.Question,
              CreatedAt = DateTime.Now,
              CreatedBy = data.CreatedBy,
              CretedByBehalfOf = data.CretedByBehalfOf
              };
            context!.SecurityQuestions!.Add(newRecord);
            context.SaveChanges();
            return Results.Created($"/securityQuestion/{data.Question}", newRecord);
        }
        else{
              var hasChanges = false;
            // Apply update to only changed fields.
            if (data.Question != null && data.Question != securityQuestion.Question) {  securityQuestion.Question=data.Question ; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != securityQuestion.ModifiedBy) { securityQuestion.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != securityQuestion.ModifiedByBehalof) { securityQuestion.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
               securityQuestion.ModifiedAt=DateTime.Now;
            if (hasChanges)
            {
                context!.SaveChanges();
                return Results.Ok(securityQuestion);
            }
            else
            {
                return Results.Problem("Not Modified.", null, 304);
            }
        
        }
        return Results.Conflict("Request or Order template is already used for another record.");
    }
     static IResult deleteSecurityQuestion(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.SecurityQuestions?.FirstOrDefault(t => t.Id == id);

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

