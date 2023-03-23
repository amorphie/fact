using amorphie.core.Base;
public record GetUserSecurityQuestionResponse(
Guid Id,
Guid SecurityQuestionId,
string SecurityAnswer,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof);
public record PostUserSecurityQuestionRequest(
Guid SecurityQuestionId,
string SecurityAnswer,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof);

public record GetCheckUserSecurityQuestionRequest(Guid SecurityQuestionId,string SecurityAnswer);