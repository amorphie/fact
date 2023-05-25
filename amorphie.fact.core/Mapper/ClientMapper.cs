using amorphie.core.Base;
using AutoMapper;
class ClientMapper : Profile
{
    public ClientMapper()
    {
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Client, ClientSaveDto>().ReverseMap();
        CreateMap<SaveClientRequest, Client>();

        CreateMap<Translation, MultilanguageText>().ReverseMap();
        CreateMap<HeaderConfiguration, HeaderConfigurationDto>().ReverseMap();
        CreateMap<Jws, JwsDto>().ReverseMap();
        CreateMap<Idempotency, IdempotencyDto>().ReverseMap();
    }
}