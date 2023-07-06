using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserTagModule 
: BaseBBTRoute<UserTagDto, UserTag, UserDBContext>
{

    public UserTagModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "Tag" };

    public override string? UrlFragment => "userTag";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapGet("search", getAllUserTagFullTextSearch);
    }

    async ValueTask<IResult> getAllUserTagFullTextSearch(
       [FromServices] UserDBContext context,
       [AsParameters] UserTagSearch dataSearch
     )
    {
        var query = context!.UserTags!
            .Skip(dataSearch.Page * dataSearch.PageSize)
            .Take(dataSearch.PageSize);

        if (!string.IsNullOrEmpty(dataSearch.Keyword))
        {
            query = query.AsNoTracking().Where(x => EF.Functions.ToTsVector("english", string.Join(" ", x.Tag, x.UserId, x.Id))
           .Matches(EF.Functions.PlainToTsQuery("english", dataSearch.Keyword)));
        }

        var userTags = query.ToList();

        if (userTags.Count() > 0)
        {
            var response = userTags.Select(x => ObjectMapper.Mapper.Map<UserTagDto>(x)).ToList();
            return Results.Ok(response);
        }

        return Results.NoContent();
    }    
}