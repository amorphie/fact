namespace amorphie.user.data;
public record GetUserTagResponse(Guid Id,string Name,Guid  UserId);
public record PostUserTagRequest(string Name,Guid  UserId);
