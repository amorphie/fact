using AutoMapper;
class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, GetUserResponse>().ReverseMap();

        CreateMap<PostUserRequest, User>();
    }
}