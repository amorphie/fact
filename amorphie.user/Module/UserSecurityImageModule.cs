using amorphie.core.Identity;
using amorphie.core.Module.minimal_api;
using amorphie.fact.core.Dtos.SecurityImage;
using amorphie.fact.data;
using amorphie.user;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserSecurityImageModule
: BaseBBTRoute<UserSecurityImageDto, UserSecurityImage, UserDBContext>
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
        routeGroupBuilder.MapPost("migrate", migrateSecurityImage);
    }

    async ValueTask<IResult> migrateSecurityImage(
        [FromServices] UserDBContext context,
       [FromBody] MigrateSecurityImageRequestDto migrateSecurityImageRequestDto
    )
    {
        var securityImage = await context!.UserSecurityImages.FirstOrDefaultAsync(q => q.Id.Equals(migrateSecurityImageRequestDto.Id));
        if(securityImage is {})
        {
            securityImage.RequireChange = migrateSecurityImageRequestDto.RequireChange;
            securityImage.ModifiedAt = migrateSecurityImageRequestDto.ModifiedAt;
            securityImage.ModifiedBy = migrateSecurityImageRequestDto.ModifiedBy;
            securityImage.ModifiedByBehalfOf = migrateSecurityImageRequestDto.ModifiedByBehalfOf;
        }
        else
        {
            securityImage = new UserSecurityImage()
            {
                Id = migrateSecurityImageRequestDto.Id,
                UserId = migrateSecurityImageRequestDto.UserId,
                SecurityImageId = migrateSecurityImageRequestDto.SecurityImageId,
                RequireChange = migrateSecurityImageRequestDto.RequireChange,
                CreatedAt = migrateSecurityImageRequestDto.CreatedAt,
                CreatedBy = migrateSecurityImageRequestDto.CreatedBy,
                CreatedByBehalfOf = migrateSecurityImageRequestDto.CreatedByBehalfOf,
                ModifiedAt = migrateSecurityImageRequestDto.ModifiedAt,
                ModifiedBy = migrateSecurityImageRequestDto.ModifiedBy,
                ModifiedByBehalfOf = migrateSecurityImageRequestDto.ModifiedByBehalfOf
            };

            await context!.UserSecurityImages.AddAsync(securityImage);
        }
        await context!.SaveChangesAsync();
        return Results.Ok();
    } 
    
    protected override async ValueTask<IResult> UpsertMethod(
    [FromServices] IMapper mapper,
    [FromServices] IValidator<UserSecurityImage> validator,
    [FromServices] UserDBContext context,
    [FromServices] IBBTIdentity bbtIdentity,
    [FromBody] UserSecurityImageDto data,
    HttpContext httpContext,
    CancellationToken token)
    {

        var userSecurityImage = context.UserSecurityImages!
          .FirstOrDefault(x => x.UserId == data.UserId);

        var user = context!.Users.FirstOrDefault(x => x.Id == data.UserId);
        var securityImage = context!.SecurityImages!.FirstOrDefault(x => x.Id == data.Id);

        if (userSecurityImage == null)
        {
            if (user.Salt != null)
            {
                var salt = Convert.FromBase64String(user.Salt);

                var password = ArgonPasswordHelper.HashPassword(securityImage.Image, salt);
                var result = Convert.ToBase64String(password);
                var newRecord = ObjectMapper.Mapper.Map<UserSecurityImage>(data);
                newRecord.CreatedAt = DateTime.UtcNow;
                newRecord.SecurityImageId = data.SecurityImageId;

                context!.UserSecurityImages!.Add(newRecord);
                context!.SaveChanges();

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
                    userSecurityImage.SecurityImageId = data.SecurityImageId;
                    hasChanges = true;
                    

                    userSecurityImage.ModifiedAt = DateTime.UtcNow;
                    userSecurityImage.ModifiedBy = bbtIdentity.UserId.Value;
                    userSecurityImage.ModifiedByBehalfOf = bbtIdentity.BehalfOfId.Value;
                }

                if (hasChanges)
                {
                    context!.SaveChanges();
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

            if (user.UserSecurityImages != null && user.UserSecurityImages.Count > 0)
            {
                var userSecurityImage = user.UserSecurityImages.OrderByDescending(i => i.CreatedAt).FirstOrDefault();
                if(userSecurityImage.Id.Equals(imageId))
                    return Results.Ok();
                else
                    return Results.Conflict();
            }

            return Results.Problem("SecurityImage definition is not found. Please check userId is exists in definitions");
        }

        return Results.Problem("User is not found");
    }
}