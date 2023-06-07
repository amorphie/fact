using AutoMapper;
public class SecurityImageMapper : Profile
{
    public SecurityImageMapper()
    {
        CreateMap<SecurityImage, SecurityImageDto>().ReverseMap();
    }
}