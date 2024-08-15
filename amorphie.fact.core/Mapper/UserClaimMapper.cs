using AutoMapper;
public class UserClaimMapper : Profile
{
    public UserClaimMapper()
    {
        CreateMap<UserClaimDto, UserClaim>().ReverseMap();
    }
}