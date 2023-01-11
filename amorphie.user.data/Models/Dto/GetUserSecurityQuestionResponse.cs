namespace amorphie.user.data;
public record GetUserSecurityQuestionResponse(Guid Id,string SecurityQuestion,Guid  UserId);
public record PostUserSecurityQuestionRequest(string SecurityQuestion,Guid  UserId);
