using System.Globalization;
using amorphie.core.Module.minimal_api;
using amorphie.fact.core.Dtos.SecurityQuestion;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SecurityQuestionModule
: BaseBBTRoute<SecurityQuestionDto, SecurityQuestion, UserDBContext>
{
    public SecurityQuestionModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "Question" };

    public override string? UrlFragment => "securityQuestion";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapGet("get/{reference}", getAllSecurityQuestions);
        routeGroupBuilder.MapPost("update/{reference}", updateSecurityQuestion);
        routeGroupBuilder.MapPost("migrate", migrateSecurityQuestion);
        routeGroupBuilder.MapGet("search", getAllSecurityQuestionFullTextSearch);
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

    async ValueTask<IResult> updateSecurityQuestion(
        [FromServices] UserDBContext context,
       [FromRoute] string reference,
       [FromBody] UpdateSecurityQuestionRequestDto updateSecurityQuestionRequestDto
    )
    {
        var user = await context!.Users.FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if (user == null)
        {
            return Results.NotFound("User Not Found");
        }

        var securityQuestion = await context!.UserSecurityQuestions.OrderByDescending(q => q.CreatedAt).FirstOrDefaultAsync();
        if (securityQuestion.Status == QuestionStatusType.Blocked)
        {
            return Results.StatusCode(457);
        }
        var culture = CultureInfo.GetCultureInfo("tr-TR");

        var passwordHasher = new PasswordHasher();
        var oldAnswer = passwordHasher.DecryptString(securityQuestion.SecurityAnswer, securityQuestion.Id.ToString());
        var isVerified = oldAnswer.ToUpper(culture).Equals(updateSecurityQuestionRequestDto.OldAnswer.Trim().ToUpper(culture));

        if (isVerified)
        {
            var newSecurityQuestion = new UserSecurityQuestion()
            {
                LastVerificationDate = DateTime.UtcNow,
                SecurityQuestionId = updateSecurityQuestionRequestDto.NewQuestionDefinitionId,
                Status = QuestionStatusType.Active,
                UserId = user.Id
            };
            newSecurityQuestion.SecurityAnswer = passwordHasher.EncryptString(updateSecurityQuestionRequestDto.NewAnswer, newSecurityQuestion.Id.ToString());
            await context!.UserSecurityQuestions.AddAsync(newSecurityQuestion);
            await context!.SaveChangesAsync();
            return Results.Ok();
        }
        else
        {
            securityQuestion.AccessFailedCount = (securityQuestion.AccessFailedCount ?? 0) + 1;
            securityQuestion.LastAccessDate = DateTime.UtcNow;
            if (securityQuestion.AccessFailedCount >= 5)
            {
                securityQuestion.Status = QuestionStatusType.Blocked;
            }
            await context!.SaveChangesAsync();
            return Results.StatusCode(456);
        }


    }

    async ValueTask<IResult> getAllSecurityQuestions(
        [FromServices] UserDBContext context,
       [FromRoute] string reference
    )
    {
        var response = new List<SecurityQuestionDto>();
        var securityQuestionDefinitions = await context.SecurityQuestions.Where(m => m.IsActive.Value).Select(m =>
                                            new amorphie.fact.core.Dtos.SecurityQuestion.SecurityQuestionDto
                                            {
                                                Id = m.Id,
                                                Description = m.Question,
                                                Key = m.Key,
                                                ValueTypeClr = m.ValueTypeClr,
                                                Priority = m.Priority.Value
                                            }).ToListAsync();

        return Results.Ok(securityQuestionDefinitions);
    }

    async ValueTask<IResult> getAllSecurityQuestionFullTextSearch(
        [FromServices] UserDBContext context,
       [AsParameters] SecurityQuestionSearch dataSearch
   )
    {
        var query = context!.SecurityQuestions!
            .Skip(dataSearch.Page * dataSearch.PageSize)
            .Take(dataSearch.PageSize);

        if (!string.IsNullOrEmpty(dataSearch.Keyword))
        {
            query = query.AsNoTracking().Where(x => EF.Functions.ToTsVector("english", string.Join(" ", x.Question, x.Id))
          .Matches(EF.Functions.PlainToTsQuery("english", dataSearch.Keyword)));
        }

        var securityQuestions = query.ToList();

        if (securityQuestions.Count() > 0)
        {
            var response = securityQuestions.Select(x => ObjectMapper.Mapper.Map<SecurityQuestionDto>(x)).ToList();
            return Results.Ok(response);
        }

        return Results.NoContent();
    }
}