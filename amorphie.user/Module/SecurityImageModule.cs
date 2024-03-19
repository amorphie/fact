using amorphie.core.Module.minimal_api;
using amorphie.fact.core.Dtos.SecurityImage;
using amorphie.fact.data;
using amorphie.user;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SecurityImageModule 
: BaseBBTRoute<SecurityImageDto, SecurityImage, UserDBContext>
{
    public SecurityImageModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "Image" };

    public override string? UrlFragment => "securityImage";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
         base.AddRoutes(routeGroupBuilder);
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
}