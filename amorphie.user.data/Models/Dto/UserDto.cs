namespace amorphie.user.data;
public record GetUserResponse(Guid Id,
string FirstName,
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
Guid CretedByBehalfOf,
Guid ModifiedByBehalof);
                             
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
Guid CretedByBehalfOf,
Guid ModifiedByBehalof,
string Salt);

public record UserCheckPasswordRequest(string Password,Guid UserId);
public record UserPasswordUpdateRequest(string oldPassword,string newPassord);