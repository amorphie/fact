using amorphie.core.Base;
using AutoMapper;
public class ClientAudienceMapper : Profile
{
    public ClientAudienceMapper()
    {
        CreateMap<ClientAudience, ClientAudienceDto>().ReverseMap();
        CreateMap<ClientAudience, ClientAudienceGetDto>();
    }
}