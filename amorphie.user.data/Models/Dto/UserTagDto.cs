namespace amorphie.user.data;
public record GetUserTagResponse(
Guid Id,
string Name,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);
public record PostUserTagRequest(
string Name,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);
