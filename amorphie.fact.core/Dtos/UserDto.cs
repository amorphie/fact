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
string Salt,
bool IsArgonHash, Guid? Id, List<string>? tags,string reason,string explanation);


public record UserCheckPasswordRequest(string Password, Guid UserId);
public record UserPasswordUpdateRequest(string oldPassword, string newPassord);
public record UserLoginRequest(string Reference, string Password);


public class UserSearch : DtoSearchBase
{
    public string? UserTag { get; set; }
}   

public class PostWorkflowUserReset
{
    public string? NewPassword { get; set; }
}