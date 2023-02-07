namespace amorphie.user.data;
public record GetUserDeviceResponse(
Guid Id,
int DeviceId,
Guid  TokenId,
Guid  ClientId,
Guid UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);

public record PostUserDeviceRequest(
int DeviceId,
Guid  TokenId,
Guid ClientId,
Guid UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);