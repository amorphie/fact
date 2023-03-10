using amorphie.core.Base;
public record GetUserResponse(
Guid Id,
string FirstName,
string LastName,
string Password,
string EMail,
Phone Phone,
string Reference,
string State,
string[] Tag,
Guid CreatedBy,
DateTime CreatedAt,
Guid ModifiedBy,
DateTime ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof);

                             
public record PostUserRequest(string FirstName,
string LastName,
string Reference,
string Password,
string EMail,
Phone Phone,
string State,
Guid CreatedBy,
DateTime? CreatedAt,
Guid ModifiedBy,
DateTime? ModifiedAt,
Guid? CreatedByBehalfOf,
Guid? ModifiedByBehalof,
string Salt);

public record UserCheckPasswordRequest(string Password,Guid UserId);
public record UserPasswordUpdateRequest(string oldPassword,string newPassord);