using AutoMapper;
public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, GetUserResponse>()
         .ConstructUsing(x=> new GetUserResponse(x.Id,x.FirstName,x.LastName,x.UserPasswords.OrderByDescending(o=>o.CreatedAt).Select(s=>s.HashedPassword).FirstOrDefault(),x.EMail,x.Phone,x.Reference,x.State,x.UserTags.Select(a=>a.Tag).ToArray<string>(),x.CreatedBy,x.CreatedAt,x.ModifiedBy,x.ModifiedAt,x.CreatedByBehalfOf,x.ModifiedByBehalfOf))
         .ReverseMap();
     
        // CreateMap<User, GetUserResponse>()
        // .ConstructUsing()
        //     .ForMember(dto => dto.Id, opt => opt.MapFrom(sh => sh.Id))
        //     .ForMember(dto => dto.FirstName, opt => opt.MapFrom(sh => sh.FirstName))
        //     .ForMember(dto => dto.LastName, opt => opt.MapFrom(sh => sh.LastName))
        //     .ForMember(dto => dto.Password, opt => opt.MapFrom(sh => sh.UserPasswords.OrderByDescending(o=>o.CreatedAt).Select(s=>s.HashedPassword).FirstOrDefault()))
        //     .ForMember(dto => dto.EMail, opt => opt.MapFrom(sh => sh.EMail))
        //     .ForMember(dto => dto.Phone, opt => opt.MapFrom(sh => sh.Phone))
        //     .ForMember(dto => dto.Reference, opt => opt.MapFrom(sh => sh.Reference))
        //     .ForMember(dto => dto.State, opt => opt.MapFrom(sh => sh.State))
        //     .ForMember(dto => dto.Tag, opt => opt.MapFrom(sh => sh.UserTags.Select(a=>a.Tag).ToArray<string>()))
        //     .ForMember(dto => dto.CreatedBy, opt => opt.MapFrom(sh => sh.CreatedBy))
        //     .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(sh => sh.CreatedAt))
        //     .ForMember(dto => dto.ModifiedBy, opt => opt.MapFrom(sh => sh.ModifiedBy))
        //  .ForMember(dto => dto.ModifiedAt, opt => opt.MapFrom(sh => sh.ModifiedAt))
        //   .ForMember(dto => dto.CreatedByBehalfOf, opt => opt.MapFrom(sh => sh.CreatedByBehalfOf))
        //   .ForMember(dto => dto.ModifiedByBehalof, opt => opt.MapFrom(sh => sh.ModifiedByBehalfOf));
                 
        // CreateMap<GetUserResponse, User>()
        //  .ConstructUsing(x=> new User(x.Id,x.FirstName,x.LastName,x.EMail,x.Phone,x.Reference,x.State,x.CreatedBy,x.CreatedAt,x.ModifiedBy,x.ModifiedAt,x.CreatedByBehalfOf,x.ModifiedByBehalof));
        CreateMap<PostUserRequest, User>();
    }
}