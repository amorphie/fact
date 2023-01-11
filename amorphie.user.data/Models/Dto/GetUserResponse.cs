namespace amorphie.user.data;
public record GetUserResponse(Guid Id,string Name,string Surname,string TcNo);
public record PostUserRequest(string Name,string Surname,string Password,string TcNo);
