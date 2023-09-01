using amorphie.core.Base;
using AutoMapper;
public class ClientTokenMapper : Profile
{
    public ClientTokenMapper()
    {
        CreateMap<ClientToken, ClientTokenDto>().ReverseMap();
        CreateMap<ClientToken, ClientTokenGetDto>();
    }
}