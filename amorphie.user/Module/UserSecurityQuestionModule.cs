using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
using amorphie.fact.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public static class UserSecurityQuestionModule
{

    static WebApplication _app = default!;

    public static void MapUserSecurityQuestionEndpoints(this WebApplication app)
    {
        _app = app;

    //     _app.MapGet("/usersecurityquestion", getAllUserSecurityQuestion)
    //     .WithOpenApi()
    //    .WithSummary("Returns saved usersecurityquestion records.")
    //    .WithDescription("Returns existing usersecurityquestion with metadata.Query parameter SecurityQuestion is can contain request or order SecurityQuestion of usersecurityquestions.")
    //    .WithTags("UserSecurityQuestion")
    //    .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
    //    .Produces(StatusCodes.Status404NotFound);

  _app.MapGet("/usersecurityquestion", getAllUserSecurityQuestionFullTextSearch)
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

        _app.MapPost("/usersecurityquestion/user/{userId}/userCheckSecurityAnswer", userCheckSecurityAnswer)
         .WithOpenApi()
        .WithSummary("Check security answer.")
        .WithDescription("Check security answer")
        .WithTags("UserSecurityQuestion")
        .Produces<GetUserSecurityQuestionResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

    }
    // static IResponse<List<GetUserSecurityQuestionResponse>> getAllUserSecurityQuestion(
    //   [FromServices] UserDBContext context,
    //   [FromQuery] Guid UserId,
    //   [FromQuery][Range(0, 100)] int page = 0,
    //   [FromQuery][Range(5, 100)] int pageSize = 100
    //   )
    // {
    //     var query = context!.UserSecurityQuestions!

    //         .Skip(page * pageSize)
    //         .Take(pageSize);

    //     if (UserId != null)
    //     {
    //         query = query.Where(t => t.UserId == UserId);
    //     }

    //     var userSecurityQuestions = query.ToList();

    //     if (userSecurityQuestions.Count() > 0)
    //     {
    //         return new Response<List<GetUserSecurityQuestionResponse>>
    //         {
    //             Data = userSecurityQuestions.Select(x => ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(x)).ToList(),
    //             Result = new Result(Status.Success, "List return successfull")
    //         };

    //     }
    //     else
    //     {
    //         return new Response<List<GetUserSecurityQuestionResponse>>
    //         {
    //             Data = null,
    //             Result = new Result(Status.Success, "No content")
    //         };
    //     }
    // }
      static IResponse<List<GetUserSecurityQuestionResponse>> getAllUserSecurityQuestionFullTextSearch(
      [FromServices] UserDBContext context,
      [FromQuery] string? SearchText,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
         var query = context!.UserSecurityQuestions!
            .Skip(page * pageSize)
            .Take(pageSize);

        if (!string.IsNullOrEmpty(SearchText))
        {
            query = query.Where(x => EF.Functions.ToTsVector("english",string.Join(" ",x.SecurityQuestionId,x.Id,x.UserId))
           .Matches(EF.Functions.PlainToTsQuery("english", SearchText)));
  
        }

        var userTags = query.ToList();

        if (userTags.Count() > 0)
        {
            return new Response<List<GetUserSecurityQuestionResponse>>
            {
                Data = userTags.Select(x => ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(x)).ToList(),
                Result = new Result(Status.Success, "List return successfull")
            };
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
    static IResponse<List<GetUserSecurityQuestionResponse>> postUserSecurityQuestion(
           [FromBody] PostUserSecurityQuestionRequest[] data,
           [FromServices] UserDBContext context
           )
    {
        var postUserQuestionRequest = new List<UserSecurityQuestion>();
        foreach (PostUserSecurityQuestionRequest item in data)
        {

            var userSecurityQuestion = context!.UserSecurityQuestions!
              .FirstOrDefault(x => x.SecurityQuestionId == item.SecurityQuestionId && x.UserId == item.UserId);
            var user = context!.Users.FirstOrDefault(x => x.Id == item.UserId);

            if (userSecurityQuestion == null)
            {
                if (user.Salt != null)
                {
                    var salt = Convert.FromBase64String(user.Salt);
                    var password = PasswordHelper.HashPassword(item.SecurityAnswer, salt);
                    var result = Convert.ToBase64String(password);
                    var newRecord = ObjectMapper.Mapper.Map<UserSecurityQuestion>(item);
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
                    postUserQuestionRequest.Add(newRecord);
                    //    return new Response<GetUserSecurityQuestionResponse>
                    //         {
                    //             Data = ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(newRecord),
                    //             Result = new Result(Status.Success, "Add successfull")
                    //         };
                }
                else
                {
                    return new Response<List<GetUserSecurityQuestionResponse>>
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
                if (item.SecurityAnswer != null)
                {
                    if (user.Salt != null)
                    {
                        var bytePassword = Convert.FromBase64String(userSecurityQuestion.SecurityAnswer);
                        var salt = Convert.FromBase64String(user.Salt);
                        var checkPassword = PasswordHelper.VerifyHash(item.SecurityAnswer, salt, bytePassword);
                        if (!checkPassword)
                        {
                            var password = PasswordHelper.HashPassword(item.SecurityAnswer, salt);
                            userSecurityQuestion.SecurityAnswer = Convert.ToBase64String(password);
                            hasChanges = true;
                        }
                    }
                    else
                    {
                        return new Response<List<GetUserSecurityQuestionResponse>>
                        {
                            Data = null,
                            Result = new Result(Status.Success, "User salt not found")
                        };
                    }
                    if (item.SecurityQuestionId != null && item.SecurityQuestionId != userSecurityQuestion.SecurityQuestionId) { userSecurityQuestion.SecurityQuestionId = item.SecurityQuestionId; hasChanges = true; }
                    if (item.UserId != null && item.UserId != userSecurityQuestion.UserId) { userSecurityQuestion.UserId = item.UserId; hasChanges = true; }
                    if (item.ModifiedBy != null && item.ModifiedBy != userSecurityQuestion.ModifiedBy) { userSecurityQuestion.ModifiedBy = item.ModifiedBy; hasChanges = true; }
                    if (item.ModifiedByBehalof != null && item.ModifiedByBehalof != userSecurityQuestion.ModifiedByBehalfOf) { userSecurityQuestion.ModifiedByBehalfOf = item.ModifiedByBehalof; hasChanges = true; }
                    userSecurityQuestion.ModifiedAt = DateTime.Now;

                    if (hasChanges)
                    {
                        context!.SaveChanges();
                        postUserQuestionRequest.Add(userSecurityQuestion);

                        // return new Response<GetUserSecurityQuestionResponse>
                        // {
                        //     Data = ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(item),
                        //     Result = new Result(Status.Success, "Kaydetme başarılı")
                        // };
                    }
                    else
                    {
                        return new Response<List<GetUserSecurityQuestionResponse>>
                        {
                            Data = postUserQuestionRequest.Select(x => ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(x)).ToList(),

                            Result = new Result(Status.Error, "Not Modified")
                        };
                    }
                }

            }

        }
        return new Response<List<GetUserSecurityQuestionResponse>>
        {
            Data = postUserQuestionRequest.Select(x => ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(x)).ToList(),

            Result = new Result(Status.Success, "Add succesfull")
        };
    }

    // return new Response<GetUserSecurityQuestionResponse>
    // {
    //     Data = ObjectMapper.Mapper.Map<GetUserSecurityQuestionResponse>(data),
    //     Result = new Result(Status.Error, "Request  is already used for another record")
    // };



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

