using amorphie.core.Module.minimal_api;
using amorphie.fact.core.Dtos.SecurityQuestion;
using amorphie.fact.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public class UserSecurityQuestionModule : BaseRoute
{
    public UserSecurityQuestionModule(WebApplication app) : base(app)
    {

    }
    const string STATE_STORE = "amorphie-cache";
    static WebApplication _app = default!;

    public override string? UrlFragment => "userSecurityQuestion";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        routeGroupBuilder.MapGet("/usersecurityquestion", getAllUserSecurityQuestionFullTextSearch)
        .WithOpenApi()
       .WithSummary("Returns saved usersecurityquestion records.")
       .WithDescription("Returns existing usersecurityquestion with metadata.Query parameter SecurityQuestion is can contain request or order SecurityQuestion of usersecurityquestions.")       
       .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapPost("/usersecurityquestion", postUserSecurityQuestion)
         .WithOpenApi()
         .WithSummary("Save UserSecurityQuestion")
         .WithDescription("Save UserSecurityQuestion")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapDelete("/usersecurityquestion/{id}", deleteUserSecurityQuestion)
        .WithOpenApi()
        .WithSummary("Deletes UserSecurityQuestion")
        .WithDescription("Delete usertag.")
        .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapPost("/usersecurityquestion/user/{userId}/userCheckSecurityAnswer", userCheckSecurityAnswer)
         .WithOpenApi()
        .WithSummary("Check security answer.")
        .WithDescription("Check security answer")
        .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapPost("migrate", migrateSecurityQuestion);
    }

    async ValueTask<IResult> deleteUserSecurityQuestion(
    [FromRoute(Name = "id")] Guid id,
    [FromServices] UserDBContext context)
    {
        var recordToDelete = context?.UserSecurityQuestions?.FirstOrDefault(t => t.Id == id);

        if (recordToDelete == null)
        {
            return Results.Problem("Not found user security question");
        }

        context!.Remove(recordToDelete);
        context.SaveChanges();

        return Results.Ok("Delete successful");
    }

    async ValueTask<IResult> migrateSecurityQuestion(
        [FromServices] UserDBContext context,
       [FromBody] MigrateSecurityQuestionRequestDto migrateSecurityQuestionRequestDto
    )
    {
        var securityQuestion = await context!.UserSecurityQuestions.FirstOrDefaultAsync(q => q.Id.Equals(migrateSecurityQuestionRequestDto.Id));
        if(securityQuestion is {})
        {
            securityQuestion.Status = migrateSecurityQuestionRequestDto.QuestionStatusType;
            securityQuestion.SecurityAnswer = migrateSecurityQuestionRequestDto.Answer;
            securityQuestion.ModifiedAt = migrateSecurityQuestionRequestDto.ModifiedAt;
            securityQuestion.ModifiedBy = migrateSecurityQuestionRequestDto.ModifiedBy;
            securityQuestion.ModifiedByBehalfOf = migrateSecurityQuestionRequestDto.ModifiedByBehalfOf;
            
        }
        else
        {
            securityQuestion = new UserSecurityQuestion()
            {
                Id = migrateSecurityQuestionRequestDto.Id,
                UserId = migrateSecurityQuestionRequestDto.UserId,
                SecurityAnswer = migrateSecurityQuestionRequestDto.Answer,
                SecurityQuestionId = migrateSecurityQuestionRequestDto.SecurityQuestionId,
                Status = migrateSecurityQuestionRequestDto.QuestionStatusType,
                CreatedAt = migrateSecurityQuestionRequestDto.CreatedAt,
                CreatedBy = migrateSecurityQuestionRequestDto.CreatedBy,
                CreatedByBehalfOf = migrateSecurityQuestionRequestDto.CreatedByBehalfOf,
                ModifiedAt = migrateSecurityQuestionRequestDto.ModifiedAt,
                ModifiedBy = migrateSecurityQuestionRequestDto.ModifiedBy,
                ModifiedByBehalfOf = migrateSecurityQuestionRequestDto.ModifiedByBehalfOf
            };

            await context!.UserSecurityQuestions.AddAsync(securityQuestion);
        }
        await context!.SaveChangesAsync();
        return Results.Ok();
    } 

    async ValueTask<IResult> userCheckSecurityAnswer(
   [FromRoute(Name = "userId")] Guid userId,
   [FromBody] GetCheckUserSecurityQuestionRequest checkUserSecurityQuestionRequest,
   [FromServices] UserDBContext context
     )
    {
        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);

        if (user != null)
        {
            var userSecurityQuestion = context!.UserSecurityQuestions!.FirstOrDefault(x => x.UserId == userId && x.SecurityQuestionId == checkUserSecurityQuestionRequest.SecurityQuestionId);
            if (userSecurityQuestion != null)
            {
                if (checkUserSecurityQuestionRequest.SecurityAnswer != null)
                {
                    if (user.Salt != null)
                    {
                        var byteQuestion = Convert.FromBase64String(userSecurityQuestion.SecurityAnswer);
                        var salt = Convert.FromBase64String(user.Salt);
                        var checkPassword = ArgonPasswordHelper.VerifyHash(checkUserSecurityQuestionRequest.SecurityAnswer, salt, byteQuestion);

                        if (checkPassword)
                        {
                            return Results.Ok("Question match");
                        }

                        return Results.Problem("Question do not match");
                    }

                    return Results.Problem("Security answer is null");
                }

                return Results.Problem("User salt not found");
            }

            return Results.Problem("SecurityAnswer definition is not found.");
        }

        return Results.Problem("User is not found.");
    }

    async ValueTask<IResult> getAllUserSecurityQuestionFullTextSearch(
    [FromServices] UserDBContext context,
    [AsParameters] UserSecurityQuestionSearch dataSearch
    )
    {
        var query = context!.UserSecurityQuestions!
           .Skip(dataSearch.Page * dataSearch.PageSize)
           .Take(dataSearch.PageSize);

        if (!string.IsNullOrEmpty(dataSearch.Keyword))
        {
            query = query.AsNoTracking().Where(x => EF.Functions.ToTsVector("english", string.Join(" ", x.SecurityQuestionId, x.Id, x.UserId))
           .Matches(EF.Functions.PlainToTsQuery("english", dataSearch.Keyword)));
        }

        var list = query.ToList();

        if (list.Count() > 0)
        {
            var response = list.Select(x => ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(x)).ToList();
            return Results.Ok(response);
        }

        return Results.NoContent();
    }

    async ValueTask<IResult> postUserSecurityQuestion(
           [FromBody] PostUserSecurityQuestionRequest[] data,
           [FromServices] UserDBContext context
           )
    {
        var postUserQuestionRequest = new List<UserSecurityQuestion>();
        foreach (PostUserSecurityQuestionRequest item in data)
        {
            var userSecurityQuestion = context!.UserSecurityQuestions!
              .FirstOrDefault(x => x.SecurityQuestionId == item.SecurityQuestionId && x.UserId == item.UserId);
            var user = context!.Users.FirstOrDefault(x => x.Id == item.UserId);

            if (userSecurityQuestion == null)
            {
                if (user.Salt != null)
                {
                    var salt = Convert.FromBase64String(user.Salt);
                    var password = ArgonPasswordHelper.HashPassword(item.SecurityAnswer, salt);
                    var result = Convert.ToBase64String(password);
                    var newRecord = ObjectMapper.Mapper.Map<UserSecurityQuestion>(item);
                    newRecord.CreatedAt = DateTime.UtcNow;
                    newRecord.SecurityAnswer = result;

                    context!.UserSecurityQuestions!.Add(newRecord);
                    context.SaveChanges();
                    postUserQuestionRequest.Add(newRecord);
                }
                else
                {
                    return Results.Problem("User salt not found");
                }
            }
            else
            {
                var hasChanges = false;
                // Apply update to only changed fields.
                if (item.SecurityAnswer != null)
                {
                    if (user.Salt != null)
                    {
                        var bytePassword = Convert.FromBase64String(userSecurityQuestion.SecurityAnswer);
                        var salt = Convert.FromBase64String(user.Salt);
                        var checkPassword = ArgonPasswordHelper.VerifyHash(item.SecurityAnswer, salt, bytePassword);
                        if (!checkPassword)
                        {
                            var password = ArgonPasswordHelper.HashPassword(item.SecurityAnswer, salt);
                            userSecurityQuestion.SecurityAnswer = Convert.ToBase64String(password);
                            hasChanges = true;
                        }
                    }
                    else
                    {
                        return Results.Problem("User salt not found");
                    }

                    if (item.SecurityQuestionId != null && item.SecurityQuestionId != userSecurityQuestion.SecurityQuestionId) { userSecurityQuestion.SecurityQuestionId = item.SecurityQuestionId; hasChanges = true; }
                    if (item.UserId != null && item.UserId != userSecurityQuestion.UserId) { userSecurityQuestion.UserId = item.UserId; hasChanges = true; }
                    if (item.ModifiedBy != null && item.ModifiedBy != userSecurityQuestion.ModifiedBy) { userSecurityQuestion.ModifiedBy = item.ModifiedBy; hasChanges = true; }
                    if (item.ModifiedByBehalof != null && item.ModifiedByBehalof != userSecurityQuestion.ModifiedByBehalfOf) { userSecurityQuestion.ModifiedByBehalfOf = item.ModifiedByBehalof; hasChanges = true; }
                    userSecurityQuestion.ModifiedAt = DateTime.Now;

                    if (hasChanges)
                    {
                        context!.SaveChanges();
                        postUserQuestionRequest.Add(userSecurityQuestion);
                    }
                    else
                    {
                        return Results.Problem("Not Modified");
                    }
                }
            }
        }

        return Results.Ok("Add succesfull");
    }
}