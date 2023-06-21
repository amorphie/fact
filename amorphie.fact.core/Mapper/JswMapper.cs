using amorphie.core.Base;
using AutoMapper;
public class JwsMapper : Profile
{
    public JwsMapper()
    {
        CreateMap<Jws, JwsDto>().ReverseMap();
        CreateMap<Jws, JwsGetDto>().ReverseMap();
    }
}