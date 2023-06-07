using AutoMapper;
public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, GetUserResponse>()
         .ConstructUsing(x=> new GetUserResponse(x.Id,x.FirstName,x.LastName,x.UserPasswords.OrderByDescending(o=>o.CreatedAt).Select(s=>s.HashedPassword).FirstOrDefault(),x.EMail,x.Phone,x.Reference,x.State,x.UserTags.Select(a=>a.Tag).ToArray<string>(),x.CreatedBy,x.CreatedAt,x.ModifiedBy,x.ModifiedAt,x.CreatedByBehalfOf,x.ModifiedByBehalfOf))
         .ReverseMap();
       
        CreateMap<PostUserRequest, User>();
    }
}