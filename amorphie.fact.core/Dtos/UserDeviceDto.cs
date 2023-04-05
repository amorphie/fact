using amorphie.core.Base;
public record GetUserDeviceResponse(
Guid Id,
int DeviceId,
Guid  TokenId,
Guid  ClientId,
Guid UserId,
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
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehaloff);