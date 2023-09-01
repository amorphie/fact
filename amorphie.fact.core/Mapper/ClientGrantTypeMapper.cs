using amorphie.core.Base;
using AutoMapper;
public class ClientGrantTypeMapper : Profile
{
    public ClientGrantTypeMapper()
    {
        CreateMap<ClientGrantType, ClientGrantTypeDto>().ReverseMap();
        CreateMap<ClientGrantType, ClientGrantTypeGetDto>();
    }
}