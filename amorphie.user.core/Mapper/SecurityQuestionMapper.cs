using AutoMapper;
class SecurityQuestionMapper : Profile
{
    public SecurityQuestionMapper()
    {
        CreateMap<SecurityQuestion, GetSecurityQuestionResponse>()
        .ConstructUsing(x=> new GetSecurityQuestionResponse(x.Id,x.Question,x.CreatedBy,x.CreatedAt,x.ModifiedBy,x.ModifiedAt,x.CreatedByBehalfOf,x.ModifiedByBehalfOf));


        CreateMap<PostSecurityQuestionRequest, SecurityQuestion>();
    }
}