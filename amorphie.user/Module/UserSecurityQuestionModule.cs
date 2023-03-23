using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
using amorphie.user.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
public static class UserSecurityQuestionModule
{

    static WebApplication _app = default!;

    public static void MapUserSecurityQuestionEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/usersecurityquestion", getAllUserSecurityQuestion)
        .WithOpenApi()
       .WithSummary("Returns saved usersecurityquestion records.")
       .WithDescription("Returns existing usersecurityquestion with metadata.Query parameter SecurityQuestion is can contain request or order SecurityQuestion of usersecurityquestions.")
       .WithTags("UserSecurityQuestion")
       .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);


        _app.MapPost("/usersecurityquestion", postUserSecurityQuestion)
         .WithOpenApi()
         .WithSummary("Save UserSecurityQuestion")
         .WithDescription("Save UserSecurityQuestion")
         .WithTags("UserSecurityQuestion")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/usersecurityquestion/{id}", deleteUserSecurityQuestion)
        .WithOpenApi()
        .WithSummary("Deletes UserSecurityQuestion")
        .WithDescription("Delete usertag.")
        .WithTags("UserSecurityQuestion")
        .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/usersecurityquestion/user/{userId}/userCheckSecurityAnswer", userCheckSecurityAnswer)
         .WithOpenApi()
        .WithSummary("Check security answer.")
        .WithDescription("Check security answer")
        .WithTags("UserSecurityQuestion")
        .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

    }
    static IResponse<List<GetUserSecurityQuestionResponse>> getAllUserSecurityQuestion(
      [FromServices] UserDBContext context,
      [FromQuery] Guid UserId,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var query = context!.UserSecurityQuestions!

            .Skip(page * pageSize)
            .Take(pageSize);

        if (UserId != null)
        {
            query = query.Where(t => t.UserId == UserId);
        }

        var userSecurityQuestions = query.ToList();

        if (userSecurityQuestions.Count() > 0)
        {
            return new Response<List<GetUserSecurityQuestionResponse>>
            {
                Data = userSecurityQuestions.Select(x => ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(x)).ToList(),
                Result = new Result(Status.Success, "List return successfull")
            };
            // return Results.Ok(userSecurityQuestions.Select(userSecurityQuestion =>
            //   new GetUserSecurityQuestionResponse(
            //    userSecurityQuestion.Id,
            //    userSecurityQuestion.SecurityQuestionId,
            //    userSecurityQuestion.SecurityAnswer,
            //    userSecurityQuestion.UserId,
            //    userSecurityQuestion.CreatedBy,
            //    userSecurityQuestion.CreatedAt,
            //    userSecurityQuestion.ModifiedBy,
            //    userSecurityQuestion.ModifiedAt,
            //    userSecurityQuestion.CreatedByBehalfOf,
            //    userSecurityQuestion.ModifiedByBehalfOf


            //     )
            // ).ToArray());
        }
        else
        {
            return new Response<List<GetUserSecurityQuestionResponse>>
            {
                Data = null,
                Result = new Result(Status.Success, "No content")
            };
        }
    }
    static IResponse<GetUserSecurityQuestionResponse> postUserSecurityQuestion(
           [FromBody] PostUserSecurityQuestionRequest data,
           [FromServices] UserDBContext context
           )
    {

        var userSecurityQuestion = context!.UserSecurityQuestions!
          .FirstOrDefault(x => x.SecurityQuestionId == data.SecurityQuestionId && x.UserId == data.UserId);
        var user = context!.Users.FirstOrDefault(x => x.Id == data.UserId);

        if (userSecurityQuestion == null)
        {
            if (user.Salt != null)
            {
                var salt = Convert.FromBase64String(user.Salt);
                var password = PasswordHelper.HashPassword(data.SecurityAnswer, salt);
                var result = Convert.ToBase64String(password);
                var newRecord = ObjectMapper.Mapper.Map<UserSecurityQuestion>(data);
                newRecord.CreatedAt = DateTime.UtcNow;
                newRecord.SecurityAnswer = result;
                // var newRecord = new UserSecurityQuestion
                // {
                //     Id = Guid.NewGuid(),
                //     SecurityAnswer = result,
                //     SecurityQuestionId = data.SecurityQuestionId,
                //     UserId = data.UserId,
                //     CreatedAt = DateTime.Now,
                //     CreatedBy = data.CreatedBy,
                //     CreatedByBehalfOf = data.CreatedByBehalfOf
                // };
                context!.UserSecurityQuestions!.Add(newRecord);
                context.SaveChanges();
                return new Response<GetUserSecurityQuestionResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(newRecord),
                    Result = new Result(Status.Success, "Add successfull")
                };
            }
            else
            {
                return new Response<GetUserSecurityQuestionResponse>
                {
                    Data = null,
                    Result = new Result(Status.Success, "User salt not found")
                };
            }
        }
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.SecurityAnswer != null)
            {
                if (user.Salt != null)
                {
                    var bytePassword = Convert.FromBase64String(userSecurityQuestion.SecurityAnswer);
                    var salt = Convert.FromBase64String(user.Salt);
                    var checkPassword = PasswordHelper.VerifyHash(data.SecurityAnswer, salt, bytePassword);
                    if (!checkPassword)
                    {
                        var password = PasswordHelper.HashPassword(data.SecurityAnswer, salt);
                        userSecurityQuestion.SecurityAnswer = Convert.ToBase64String(password);
                        hasChanges = true;
                    }
                }
                else
                {
                    return new Response<GetUserSecurityQuestionResponse>
                    {
                        Data = null,
                        Result = new Result(Status.Success, "User salt not found")
                    };
                }
                if (data.SecurityQuestionId != null && data.SecurityQuestionId != userSecurityQuestion.SecurityQuestionId) { userSecurityQuestion.SecurityQuestionId = data.SecurityQuestionId; hasChanges = true; }
                if (data.UserId != null && data.UserId != userSecurityQuestion.UserId) { userSecurityQuestion.UserId = data.UserId; hasChanges = true; }
                if (data.ModifiedBy != null && data.ModifiedBy != userSecurityQuestion.ModifiedBy) { userSecurityQuestion.ModifiedBy = data.ModifiedBy; hasChanges = true; }
                if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != userSecurityQuestion.ModifiedByBehalfOf) { userSecurityQuestion.ModifiedByBehalfOf = data.ModifiedByBehalof; hasChanges = true; }
                userSecurityQuestion.ModifiedAt = DateTime.Now;

                if (hasChanges)
                {
                    context!.SaveChanges();
                    return new Response<GetUserSecurityQuestionResponse>
                    {
                        Data = ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(user),
                        Result = new Result(Status.Success, "Kaydetme başarılı")
                    };
                }
                else
                {
                    return new Response<GetUserSecurityQuestionResponse>
                    {
                        Data = ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(data),
                        Result = new Result(Status.Error, "Not Modified")
                    };
                }
            }

        }
        return new Response<GetUserSecurityQuestionResponse>
        {
            Data = ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(data),
            Result = new Result(Status.Error, "Request  is already used for another record")
        };
    }
    static IResponse deleteUserSecurityQuestion(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.UserSecurityQuestions?.FirstOrDefault(t => t.Id == id);

        if (recordToDelete == null)
        {
            return new NoDataResponse
            {
                Result = new Result(Status.Success, "Not found user security question")
            };
        }
        else
        {
            context!.Remove(recordToDelete);
            context.SaveChanges();
            return new NoDataResponse
            {
                Result = new Result(Status.Error, "Delete successful")
            };
        }
    }
    static IResponse userCheckSecurityAnswer(
       [FromRoute(Name = "userId")] Guid userId,
       [FromBody] GetCheckUserSecurityQuestionRequest checkUserSecurityQuestionRequest,
       [FromServices] UserDBContext context
         )
    {
        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            var userSecurityQuestion = context!.UserSecurityQuestions!.FirstOrDefault(x => x.UserId == userId && x.SecurityQuestionId == checkUserSecurityQuestionRequest.SecurityQuestionId);
            if (userSecurityQuestion != null)
            {
                if (checkUserSecurityQuestionRequest.SecurityAnswer != null)
                {
                    if (user.Salt != null)
                    {
                        var byteQuestion = Convert.FromBase64String(userSecurityQuestion.SecurityAnswer);
                        var salt = Convert.FromBase64String(user.Salt);
                        var checkPassword = PasswordHelper.VerifyHash(checkUserSecurityQuestionRequest.SecurityAnswer, salt, byteQuestion);
                        if (checkPassword)
                        {
                            return new NoDataResponse
                            {
                                Result = new Result(Status.Success, "Question match")
                            };
                        }
                        else
                        {
                            return new NoDataResponse
                            {

                                Result = new Result(Status.Error, "Question do not match")
                            };
                        }
                    }
                    else
                    {
                        return new NoDataResponse
                        {

                            Result = new Result(Status.Error, "Security answer is null")
                        };

                    }
                }
                else
                {

                    return new NoDataResponse
                    {
                        Result = new Result(Status.Success, "User salt not found")
                    };
                }

            }
            else
            {
                return new NoDataResponse
                {

                    Result = new Result(Status.Error, "SecurityAnswer definition is not found.")
                };

            }
        }
        else
        {
            return new NoDataResponse
            {

                Result = new Result(Status.Error, "User is not found")
            };
        }

    }
}

