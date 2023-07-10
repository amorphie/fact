using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using AutoMapper;
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

        routeGroupBuilder.MapGet("search", getAllSecurityQuestionFullTextSearch);
    }

    async ValueTask<IResult> getAllSecurityQuestionFullTextSearch(
        [FromServices] UserDBContext context,
        [AsParameters] SecurityQuestionSearch dataSearch,
        [FromServices] IMapper mapper,
        CancellationToken token
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

        IList<SecurityQuestion> resultList = await query.ToListAsync(token);

        return (resultList != null && resultList.Count > 0)
            ? Results.Ok(mapper.Map<IList<SecurityQuestionDto>>(resultList))
            : Results.NoContent();
    }
}