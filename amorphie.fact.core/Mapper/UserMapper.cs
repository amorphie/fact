using AutoMapper;
public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, GetUserResponse>()
         .ConstructUsing(x => new GetUserResponse(x.Id, x.FirstName, x.LastName, x.UserPasswords.OrderByDescending(o => o.CreatedAt).Select(s => s.HashedPassword).FirstOrDefault(), x.EMail, x.Phone, x.Reference, x.State, x.UserTags.Select(a => a.Tag).ToArray<string>(), x.CreatedBy, x.CreatedAt, x.ModifiedBy, x.ModifiedAt, x.CreatedByBehalfOf, x.ModifiedByBehalfOf))
         .ReverseMap();

        CreateMap<PostUserRequest, User>();
        CreateMap<PostWorkflowDtoUser, PostUserRequest>()
         .ConstructUsing(x => new PostUserRequest(x.data.FirstName, x.data.LastName, x.data.Reference, x.data.Password, x.data.EMail,
         x.data.Phone, x.data.State, x.data.CreatedBy, x.data.CreatedAt, x.data.ModifiedBy, x.data.ModifiedAt,
         x.data.CreatedByBehalfOf, x.data.ModifiedByBehalof, x.data.Salt, x.data.IsArgonHash, x.recordId, x.data.tags, x.data.reason, x.data.explanation))
         .ReverseMap();
    }
}