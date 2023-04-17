
public record GetUserDeviceResponse(
Guid Id,
int DeviceId,
Guid  TokenId,
Guid  ClientId,
Guid UserId,
int Status,
Guid CreatedBy,
DateTime CreatedAt,
Guid ModifiedBy,
DateTime ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof);

public record PostUserDeviceRequest(
int DeviceId,
Guid  TokenId,
Guid ClientId,
Guid UserId,
int Status,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehaloff);