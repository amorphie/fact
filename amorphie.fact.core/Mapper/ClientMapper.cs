using amorphie.core.Base;
using AutoMapper;
public class ClientMapper : Profile
{
    public ClientMapper()
    {
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Client, ClientGetDto>();

        CreateMap<Translation, MultilanguageText>().ReverseMap();
        CreateMap<HeaderConfiguration, HeaderConfigurationDto>().ReverseMap();
        CreateMap<Jws, JwsDto>().ReverseMap();
        CreateMap<Idempotency, IdempotencyDto>().ReverseMap();
    }
}