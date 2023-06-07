using AutoMapper;
public class UserDeviceMapper : Profile
{
    public UserDeviceMapper()
    {
        CreateMap<UserDevice, UserDeviceDto>().ReverseMap();
    }
}