using amorphie.core.Base;
using AutoMapper;
public class ClientFlowMapper : Profile
{
    public ClientFlowMapper()
    {
        CreateMap<ClientFlow, ClientFlowDto>().ReverseMap(); 
    }
}