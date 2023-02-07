namespace amorphie.user.data;
public record GetUserSecurityImageResponse(
Guid Id,
string SecurityImage,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);
public record PostUserSecurityImageRequest(
string SecurityImage,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);