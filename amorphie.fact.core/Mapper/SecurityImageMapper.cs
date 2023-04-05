using AutoMapper;
class SecurityImageMapper : Profile
{
    public SecurityImageMapper()
    {
        CreateMap<SecurityImage, GetSecurityImageResponse>().ReverseMap();

        CreateMap<PostSecurityImageRequest, SecurityImage>();
    }
}