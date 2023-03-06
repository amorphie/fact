using AutoMapper;
class UserTagMapper : Profile
{
    public UserTagMapper()
    {
        CreateMap<UserTag, GetUserTagResponse>()
        .ConstructUsing(x=> new GetUserTagResponse(x.Id,x.Tag,x.UserId,x.CreatedBy,x.CreatedAt,x.ModifiedBy,x.ModifiedAt,x.CreatedByBehalfOf,x.ModifiedByBehalfOf));

        CreateMap<PostUserTagRequest, UserTag>();
    }
}