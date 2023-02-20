using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class SecurityQuestionModule
{

    static WebApplication _app = default!;

    public static void MapSecurityQuestionEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/securityquestion", getAllSecurityQuestion)
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

        _app.MapPost("/securityquestion", postSecurityQuestion)
         .WithOpenApi()
         .WithSummary("Save securityquestion")
         .WithTags("SecurityQuestion")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/securityquestion/{id}", deleteSecurityQuestion)
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
             query= query.Where(x => x.Question.Contains(Question));
        }

        var securityQuestions = query.ToList();

        if (securityQuestions.Count() > 0)
        {
            return Results.Ok(securityQuestions.Select(securityQuestion =>
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
          .FirstOrDefault(x => x.Question == data.Question);
        if (securityQuestion == null)
        {
            var newRecord = new SecurityQuestion
            {
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
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.Question != null && data.Question != securityQuestion.Question) { securityQuestion.Question = data.Question; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != securityQuestion.ModifiedBy) { securityQuestion.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != securityQuestion.ModifiedByBehalof) { securityQuestion.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
            securityQuestion.ModifiedAt = DateTime.Now;
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

