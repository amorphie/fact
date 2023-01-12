namespace amorphie.user.data;
public record GetUserResponse(Guid Id,string Name,string Surname,string TcNo,string Password,int State,DateTime LastLoginDate,DateTime ModifiedDate);
public record PostUserRequest(string Name,string Surname,string Password,string TcNo);
