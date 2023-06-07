using AutoMapper;
public class SecurityQuestionMapper : Profile
{
    public SecurityQuestionMapper()
    {
       CreateMap<SecurityQuestion, SecurityQuestionDto>().ReverseMap();
    }
}