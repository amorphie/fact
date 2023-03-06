using AutoMapper;
class UserSecurityImageMapper : Profile
{
    public UserSecurityImageMapper()
    {
        CreateMap<UserSecurityImage, GetUserSecurityImageResponse>()
         .ConstructUsing(x=> new GetUserSecurityImageResponse(x.Id,x.SecurityImage,x.UserId,x.CreatedAt,x.CreatedBy,x.CreatedByBehalfOf,x.ModifiedAt,x.ModifiedBy,x.ModifiedByBehalfOf));

        CreateMap<PostUserSecurityImageRequest, UserSecurityImage>();
    }
}