
public record GetUserSecurityImageResponse(
Guid Id,
string SecurityImage,
Guid UserId,
DateTime CreatedAt,
Guid CreatedBy,
Guid? CreatedByBehalfOf,
DateTime ModifiedAt,
Guid ModifiedBy,
Guid? ModifiedByBehalfOf);


public record PostUserSecurityImageRequest(
Guid SecurityImageId,
Guid UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof);
