using amorphie.core.Repository;
using amorphie.fact.data;
using amorphie.user;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserSecurityImageModule : BaseUserSecurityImageModule<UserSecurityImageDto, UserSecurityImage, UserSecurityImageValidator>
{
    public UserSecurityImageModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { };

    public override string? UrlFragment => "userSecurityImage";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapGet("/user/{userId}/image/{imageId}/userCheckImage", userCheckImage);
    }

    protected override async ValueTask<IResult> Upsert([FromServices] IMapper mapper,
      [FromServices] UserSecurityImageValidator validator,
      [FromServices] IBBTRepository<UserSecurityImage, UserDBContext> repository,
      [FromBody] UserSecurityImageDto data)
    {

        var userSecurityImage = repository.DbContext.UserSecurityImages!
          .FirstOrDefault(x => x.UserId == data.UserId);

        var user = repository.DbContext!.Users.FirstOrDefault(x => x.Id == data.UserId);
        var securityImage = repository.DbContext!.SecurityImages!.FirstOrDefault(x => x.Id == data.Id);

        if (userSecurityImage == null)
        {
            if (user.Salt != null)
            {
                var salt = Convert.FromBase64String(user.Salt);

                var password = ArgonPasswordHelper.HashPassword(securityImage.Image, salt);
                var result = Convert.ToBase64String(password);
                var newRecord = ObjectMapper.Mapper.Map<UserSecurityImage>(data);
                newRecord.CreatedAt = DateTime.UtcNow;
                newRecord.SecurityImage = result;

                repository.DbContext!.UserSecurityImages!.Add(newRecord);
                repository.DbContext.SaveChanges();

                return Results.Ok(newRecord);
            }

            return Results.Problem("User salt not found");
        }
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (securityImage.Image != null)
            {
                if (user.Salt != null)
                {

                    var bytePassword = Convert.FromBase64String(userSecurityImage.SecurityImage);
                    var salt = Convert.FromBase64String(user.Salt);
                    var checkPassword = ArgonPasswordHelper.VerifyHash(securityImage.Image, salt, bytePassword);

                    if (!checkPassword)
                    {
                        var password = ArgonPasswordHelper.HashPassword(securityImage.Image, salt);
                        userSecurityImage.SecurityImage = Convert.ToBase64String(password);
                        hasChanges = true;
                    }

                    if (data.ModifiedByBehalfOf != null && data.ModifiedByBehalfOf != userSecurityImage.ModifiedByBehalfOf) { userSecurityImage.ModifiedByBehalfOf = data.ModifiedByBehalfOf; hasChanges = true; }
                    if (data.ModifiedBy != null && data.ModifiedBy != userSecurityImage.ModifiedBy) { userSecurityImage.ModifiedBy = data.ModifiedBy; hasChanges = true; }
                    userSecurityImage.ModifiedAt = DateTime.Now;
                }

                if (hasChanges)
                {
                    repository.DbContext!.SaveChanges();
                }

                return Results.NoContent();
            }
            else
            {
                return Results.Problem("User salt not found");
            }
        }
    }

    async ValueTask<IResult> userCheckImage(
       [FromRoute(Name = "userId")] Guid userId,
       [FromRoute(Name = "imageId")] Guid imageId,
        [FromServices] UserDBContext context
         )
    {
        var user = context!.Users!
      .Include(x => x.UserSecurityImages)
      .FirstOrDefault(x => x.Id == userId);

        if (user != null)
        {
            var securityImage = context!.SecurityImages!.FirstOrDefault(x => x.Id == imageId);

            if (user.UserSecurityImages != null && user.UserSecurityImages.Count > 0)
            {
                var userSecurityImage = user.UserSecurityImages.FirstOrDefault();
                var byteImage = Convert.FromBase64String(userSecurityImage.SecurityImage);
                var salt = Convert.FromBase64String(user.Salt);
                var checkPassword = ArgonPasswordHelper.VerifyHash(securityImage.Image, salt, byteImage);

                if (checkPassword)
                {
                    return Results.Ok("Image match");
                }

                return Results.Problem("Image do not match");
            }

            return Results.Problem("SecurityImage definition is not found. Please check userId is exists in definitions");
        }

        return Results.Problem("User is not found");
    }
}