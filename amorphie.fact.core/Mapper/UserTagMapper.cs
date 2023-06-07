using AutoMapper;
public class UserTagMapper : Profile
{
    public UserTagMapper()
    {
          CreateMap<UserTag, UserTagDto>().ReverseMap();
    }
}