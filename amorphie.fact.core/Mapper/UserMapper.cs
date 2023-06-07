using AutoMapper;
public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, GetUserResponse>()
         .ConstructUsing(x=> new GetUserResponse(x.Id,x.FirstName,x.LastName,x.UserPasswords.OrderByDescending(o=>o.CreatedAt).Select(s=>s.HashedPassword).FirstOrDefault(),x.EMail,x.Phone,x.Reference,x.State,x.UserTags.Select(a=>a.Tag).ToArray<string>(),x.CreatedBy,x.CreatedAt,x.ModifiedBy,x.ModifiedAt,x.CreatedByBehalfOf,x.ModifiedByBehalfOf))
         .ReverseMap();
       
        CreateMap<PostUserRequest, User>();
        CreateMap<PostWorkflowDto, PostUserRequest>()
         .ConstructUsing(x=> new PostUserRequest(x.entityData.FirstName,x.entityData.LastName,x.entityData.Reference,x.entityData.Password,x.entityData.EMail,
         x.entityData.Phone,x.entityData.State,x.entityData.CreatedBy,x.entityData.CreatedAt,x.entityData.ModifiedBy,x.entityData.ModifiedAt,
         x.entityData.CreatedByBehalfOf,x.entityData.ModifiedByBehalof,x.entityData.Salt,x.entityData.IsArgonHash,x.recordId))
         .ReverseMap();
    }
}