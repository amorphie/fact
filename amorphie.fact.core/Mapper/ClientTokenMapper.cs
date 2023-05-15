using amorphie.core.Base;
using AutoMapper;
class ClientTokenMapper : Profile
{
    public ClientTokenMapper()
    {
        CreateMap<ClientToken, ClientTokenDto>().ReverseMap();        
        CreateMap<SaveClientTokenRequest, ClientToken>();
    }
}