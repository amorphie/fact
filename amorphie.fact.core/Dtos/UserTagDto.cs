
public record GetUserTagResponse(
Guid Id,
string Tag,
Guid  UserId,
Guid CreatedBy,
DateTime CreatedAt,
Guid ModifiedBy,
DateTime ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof);

public record PostUserTagRequest(
string Tag,
Guid  UserId,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof);   
