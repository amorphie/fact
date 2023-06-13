using amorphie.core.Base;
using AutoMapper;
public class DeactiveDefinitionMapper : Profile
{
    public DeactiveDefinitionMapper()
    {
        CreateMap<DeactiveDefinition, DeactiveDefinitionDto>().ReverseMap(); 
    }
}