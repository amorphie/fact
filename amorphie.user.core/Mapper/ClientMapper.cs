using AutoMapper;
class ClientMapper : Profile
{
    public ClientMapper()
    {
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<SaveClientRequest, Client>();
    }
}