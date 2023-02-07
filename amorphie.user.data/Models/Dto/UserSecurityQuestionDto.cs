namespace amorphie.user.data;
public record GetUserSecurityQuestionResponse(
Guid Id,
Guid SecurityQuestionId,
string SecurityAnswer,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);
public record PostUserSecurityQuestionRequest(
Guid SecurityQuestionId,
string SecurityAnswer,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);
