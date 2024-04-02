using amorphie.core.Identity;
using amorphie.core.Module.minimal_api;
using amorphie.fact.core.Dtos.SecurityImage;
using amorphie.fact.core.Helper;
using amorphie.fact.data;
using amorphie.user;
using AutoMapper;
using FluentValidation;
using Google.Api;
using Microsoft.AspNetCore.Http.HttpResults;
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
        routeGroupBuilder.MapPut("/user/{reference}",updateSecurityImage);
        routeGroupBuilder.MapGet("/user/{reference}",getSecurityImages);
        routeGroupBuilder.MapPost("migrate", migrateSecurityImage);
        routeGroupBuilder.MapPost("migrateImages", migrateSecurityImages);
    }

    async ValueTask<IResult> updateSecurityImage(
        [FromServices] UserDBContext context,
       [FromRoute] string reference,
       [FromBody] UpdateSecurityImageDto updateSecurityImageDto
    )
    {
        var user = await context!.Users.FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if (user == null)
        {
            return Results.NotFound("User Not Found");
        }

        var securityImage = new UserSecurityImage{
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user.Id,
            CreatedByBehalfOf = null,
            RequireChange = null,
            UserId = user.Id,
            SecurityImageId = updateSecurityImageDto.ImageId
        };

        await context!.AddAsync(securityImage);
        await context!.SaveChangesAsync();

        return Results.Ok();
    }
    async ValueTask<IResult> getSecurityImages(
        HttpContext httpContext,
        [FromServices] UserDBContext context,
       [FromRoute] string reference
    )
    {
        var user = await context!.Users.FirstOrDefaultAsync(u => u.Reference.Equals(reference));
        if (user == null)
        {
            return Results.NotFound("User Not Found");
        }

        var securityImages = await context!.SecurityImages.ToListAsync();
        var userSecurityImage = await context!.UserSecurityImages.OrderByDescending(i => i.CreatedAt).FirstOrDefaultAsync(i => i.UserId == user.Id);

        var response = new List<amorphie.fact.core.Dtos.SecurityImage.SecurityImageDto>();

        foreach (var item in securityImages)
        {
            response.Add(new amorphie.fact.core.Dtos.SecurityImage.SecurityImageDto{
                Id = item.Id,
                ImagePath = item.Image,
                IsSelected = item.Id.Equals(userSecurityImage?.SecurityImageId),
                Title = LangHelper.GetLang(httpContext).Equals("tr") ? item.TrTitle : item.EnTitle
            });
        }

        return Results.Json(response, new System.Text.Json.JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
    }

    async ValueTask<IResult> migrateSecurityImages(
        [FromServices] UserDBContext context,
       [FromBody] List<SecurityImageRequestDto> securityImageRequestDtos
    )
    {
        foreach (var image in securityImageRequestDtos)
        {
            var img = await context.SecurityImages.FirstOrDefaultAsync(s => s.Id.Equals(image.Id));
            if(img is not {})
            {
                await context.SecurityImages.AddAsync(new SecurityImage(){
                    CreatedAt = image.CreatedAt,
                    CreatedBy = image.CreatedBy,
                    CreatedByBehalfOf = image.CreatedByBehalfOf,
                    ModifiedAt = image.ModifiedAt,
                    ModifiedBy = image.ModifiedBy,
                    ModifiedByBehalfOf = image.ModifiedByBehalfOf,
                    EnTitle = image.EnTitle,
                    TrTitle = image.TrTitle,
                    Id = image.Id,
                    Image = image.Image
                });
            }
        }

        await context.SaveChangesAsync();
        return Results.Ok();
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