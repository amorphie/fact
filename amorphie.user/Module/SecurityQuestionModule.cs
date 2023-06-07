using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SecurityQuestionModule : BaseSecurityQuestionModule<SecurityQuestionDto, SecurityQuestion, SecurityQuestionValidator>
{
    public SecurityQuestionModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "Question" };

    public override string? UrlFragment => "securityQuestion";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapGet("search", getAllSecurityQuestionFullTextSearch);
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