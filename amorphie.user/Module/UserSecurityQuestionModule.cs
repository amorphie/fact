using System.ComponentModel.DataAnnotations;
using amorphie.user.data;
using Microsoft.AspNetCore.Mvc;

public static class UserSecurityQuestionModule
{
   
    static WebApplication _app = default!;

    public static void MapUserSecurityQuestionEndpoints(this WebApplication app)
    {
       _app = app;

         _app.MapGet("/userSecurityQuestion", getAllUserSecurityQuestion)
         .WithOpenApi()
        .WithSummary("Returns saved usersecurityquestion records.")
        .WithDescription("Returns existing usersecurityquestion with metadata.Query parameter SecurityQuestion is can contain request or order SecurityQuestion of usersecurityquestions.")
        .WithTags("UserSecurityQuestion")
        .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
     

       _app.MapPost("/userSecurityQuestion", postUserSecurityQuestion)
        .WithOpenApi()
        .WithSummary("Save UserSecurityQuestion")
        .WithDescription("Save UserSecurityQuestion")
        .WithTags("UserSecurityQuestion")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

       _app.MapDelete("/userSecurityQuestion/{id}", deleteUserSecurityQuestion)
       .WithOpenApi()
       .WithSummary("Deletes UserSecurityQuestion")
       .WithDescription("Delete usertag.")
       .WithTags("UserSecurityQuestion")
       .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

    }
      static IResult getAllUserSecurityQuestion(
        [FromServices] UserDBContext context,
        [FromQuery] Guid UserId,
        [FromQuery][Range(0, 100)] int page = 0,
        [FromQuery][Range(5, 100)] int pageSize = 100
        )
    {
        var query = context!.UserSecurityQuestions!
            
            .Skip(page * pageSize)
            .Take(pageSize);

        if (UserId!=null)
        {
            query.Where(t => t.UserId==UserId);
        }

        var userSecurityQuestions = query.ToList();

        if (userSecurityQuestions.Count() > 0)
        {
            return Results.Ok(userSecurityQuestions.Select(userSecurityQuestion =>
              new GetUserSecurityQuestionResponse(
               userSecurityQuestion.Id,
               userSecurityQuestion.SecurityQuestionId,
               userSecurityQuestion.SecurityAnswer,
               userSecurityQuestion.UserId,
               userSecurityQuestion.CreatedBy,
               userSecurityQuestion.CreatedAt,
               userSecurityQuestion.ModifiedBy,
               userSecurityQuestion.ModifiedAt,
               userSecurityQuestion.CretedByBehalfOf,
               userSecurityQuestion.ModifiedByBehalof
               
               
                )
            ).ToArray());
        }
        else
            return Results.NoContent();
    }
 static async Task<IResult> postUserSecurityQuestion(
        [FromBody] PostUserSecurityQuestionRequest data,
        [FromServices] UserDBContext context
        )
    {
       
        var userSecurityQuestion = context!.UserSecurityQuestions!
          .FirstOrDefault(x => x.SecurityQuestionId==data.SecurityQuestionId && x.UserId==data.UserId );
        var user=context!.Users.FirstOrDefault(x=>x.Id==data.UserId);

        if (userSecurityQuestion== null)
        {
           
            var salt = Convert.FromBase64String(user.Salt);
            var password = PasswordHelper.HashPassword(data.SecurityAnswer, salt);
            var result = Convert.ToBase64String(password);
   
            var newRecord = new UserSecurityQuestion {
                Id = Guid.NewGuid(),
                SecurityAnswer =result,
                SecurityQuestionId=data.SecurityQuestionId,
                UserId=data.UserId,
                CreatedAt = DateTime.Now,
                CreatedBy = data.CreatedBy,
                CretedByBehalfOf = data.CretedByBehalfOf
                 };
            context!.UserSecurityQuestions!.Add(newRecord);
            context.SaveChanges();
            return Results.Created($"/userSecurityQuestion/{data.UserId}", newRecord);
        }
        else{
              var hasChanges = false;
            // Apply update to only changed fields.
            if (data.SecurityAnswer != null)
            {
                var bytePassword = Convert.FromBase64String(userSecurityQuestion.SecurityAnswer);
                var salt = Convert.FromBase64String(user.Salt);
                var checkPassword = PasswordHelper.VerifyHash(data.SecurityAnswer, salt, bytePassword);
                if (!checkPassword)
                {
                    var password = PasswordHelper.HashPassword(data.SecurityAnswer, salt);
                    user.Password = Convert.ToBase64String(password);
                    hasChanges=true;
                }
            if (data.SecurityQuestionId != null && data.SecurityQuestionId != userSecurityQuestion.SecurityQuestionId) {  userSecurityQuestion.SecurityQuestionId=data.SecurityQuestionId ; hasChanges = true; }
            if (data.UserId != null && data.UserId != userSecurityQuestion.UserId) { userSecurityQuestion.UserId = data.UserId; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != userSecurityQuestion.ModifiedBy) { userSecurityQuestion.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != userSecurityQuestion.ModifiedByBehalof) { userSecurityQuestion.ModifiedByBehalof = data.ModifiedByBehalof; hasChanges = true; }
                userSecurityQuestion.ModifiedAt=DateTime.Now;   
            }
            if (hasChanges)
            {
                context!.SaveChanges();
                return Results.Ok(userSecurityQuestion);
            }
            else
            {
                return Results.Problem("Not Modified.", null, 304);
            }
        
        }
        return Results.Conflict("Request or Order template is already used for another record.");
    }
     static IResult deleteUserSecurityQuestion(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.UserSecurityQuestions?.FirstOrDefault(t => t.Id == id);

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

