using AutoMapper;
class UserDeviceMapper : Profile
{
    public UserDeviceMapper()
    {
        CreateMap<UserDevice, GetUserDeviceResponse>()
          .ConstructUsing(x=> new GetUserDeviceResponse(x.Id,x.DeviceId,x.TokenId,x.ClientId,x.UserId,x.Status,x.CreatedBy,x.CreatedAt,x.ModifiedBy,x.ModifiedAt,x.CreatedByBehalfOf,x.ModifiedByBehalfOf));

        CreateMap<PostUserDeviceRequest, UserDevice>();
    }
}