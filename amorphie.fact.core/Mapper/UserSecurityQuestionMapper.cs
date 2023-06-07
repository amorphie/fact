using AutoMapper;
public class UserSecurityQuestionMapper : Profile
{
    public UserSecurityQuestionMapper()
    {
       CreateMap<UserSecurityQuestion, GetUserSecurityQuestionResponse>()
        .ConstructUsing(x=> new GetUserSecurityQuestionResponse(x.Id,x.SecurityQuestionId,x.SecurityAnswer,x.UserId,x.CreatedBy,x.CreatedAt,x.ModifiedBy,x.ModifiedAt,x.CreatedByBehalfOf,x.ModifiedByBehalfOf));

        CreateMap<PostUserSecurityQuestionRequest, UserSecurityQuestion>();
    }
}