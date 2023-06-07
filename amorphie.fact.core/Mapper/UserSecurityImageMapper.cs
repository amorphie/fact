using AutoMapper;
public class UserSecurityImageMapper : Profile
{
    public UserSecurityImageMapper()
    {
        CreateMap<UserSecurityImage, UserSecurityImageDto>().ReverseMap();
    }
}