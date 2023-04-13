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

public static class SecurityQuestionModule
{

    static WebApplication _app = default!;

    public static void MapSecurityQuestionEndpoints(this WebApplication app)
    {
        _app = app;

    //     _app.MapGet("/securityquestion", getAllSecurityQuestion)
    //    .WithTags("SecurityQuestion")
    //    .WithOpenApi(operation =>
    //    {
    //        operation.Summary = "Returns saved usertag records.";
    //        operation.Parameters[0].Description = "Filtering parameter. Given **securityQuestion** is used to filter securityQuestions.";
    //        operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
    //        return operation;
    //    })
    //    .Produces<GetUserResponse[]>(StatusCodes.Status200OK)
    //    .Produces(StatusCodes.Status204NoContent);

        _app.MapGet("/securityquestion", getAllSecurityQuestionFullTextSearch)
       .WithTags("SecurityQuestion")
       .WithOpenApi(operation =>
       {
           operation.Summary = "Returns saved usertag records.";
           operation.Parameters[0].Description = "Filtering parameter. Given **securityQuestion** is used to filter securityQuestions.";
           operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
           return operation;
       })
       .Produces<GetUserResponse[]>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status204NoContent);

        _app.MapPost("/securityquestion", postSecurityQuestion)
         .WithOpenApi()
         .WithSummary("Save securityquestion")
         .WithTags("SecurityQuestion")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/securityquestion/{id}", deleteSecurityQuestion)
        .WithOpenApi()
        .WithSummary("Deletes securityquestion")
        .WithDescription("Delete securityquestion.")
        .WithTags("SecurityQuestion")
         .Produces<GetUserDeviceResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);



    }
    // static IResponse<List<GetSecurityQuestionResponse>> getAllSecurityQuestion(
    //   [FromServices] UserDBContext context,
    //   [FromQuery] string? Question,
    //   [FromQuery][Range(0, 100)] int page = 0,
    //   [FromQuery][Range(5, 100)] int pageSize = 100
    //   )
    // {
    //     var query = context!.SecurityQuestions!
    //         .Skip(page * pageSize)
    //         .Take(pageSize);

    //     if (!string.IsNullOrEmpty(Question))
    //     {
    //          query= query.Where(x => x.Question.Contains(Question));
    //     }

    //     var securityQuestions = query.ToList();

    //     if (securityQuestions.Count() > 0)
    //     {
    //            return new Response<List<GetSecurityQuestionResponse>>
    //         {
    //             Data = securityQuestions.Select(x => ObjectMapper.Mapper.Map<GetSecurityQuestionResponse>(x)).ToList(),
    //             Result = new Result(Status.Success, "List return successfull")
    //         };
    //         // return Results.Ok(securityQuestions.Select(securityQuestion =>
    //         //   new GetSecurityQuestionResponse(
    //         //    securityQuestion.Id,
    //         //    securityQuestion.Question,
    //         //    securityQuestion.CreatedBy,
    //         //    securityQuestion.CreatedAt,
    //         //    securityQuestion.ModifiedBy,
    //         //    securityQuestion.ModifiedAt,
    //         //    securityQuestion.CreatedByBehalfOf,
    //         //    securityQuestion.ModifiedByBehalfOf
    //         //     )
    //         // ).ToArray());
    //     }
    //       else
    //     {
    //         return new Response<List<GetSecurityQuestionResponse>>
    //         {
    //             Data = null,
    //             Result = new Result(Status.Success, "No content")
    //         };
    //     }
    // }
      static IResponse<List<GetSecurityQuestionResponse>> getAllSecurityQuestionFullTextSearch(
      [FromServices] UserDBContext context,
      [FromQuery] string? SearchText,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var query = context!.SecurityQuestions!
            .Skip(page * pageSize)
            .Take(pageSize);

        if (!string.IsNullOrEmpty(SearchText))
        {
             query = query.Where(x => EF.Functions.ToTsVector("english",string.Join(" ",x.Question,x.Id))
           .Matches(EF.Functions.PlainToTsQuery("english", SearchText)));
        }

        var securityQuestions = query.ToList();

        if (securityQuestions.Count() > 0)
        {
               return new Response<List<GetSecurityQuestionResponse>>
            {
                Data = securityQuestions.Select(x => ObjectMapper.Mapper.Map<GetSecurityQuestionResponse>(x)).ToList(),
                Result = new Result(Status.Success, "List return successfull")
            };
            // return Results.Ok(securityQuestions.Select(securityQuestion =>
            //   new GetSecurityQuestionResponse(
            //    securityQuestion.Id,
            //    securityQuestion.Question,
            //    securityQuestion.CreatedBy,
            //    securityQuestion.CreatedAt,
            //    securityQuestion.ModifiedBy,
            //    securityQuestion.ModifiedAt,
            //    securityQuestion.CreatedByBehalfOf,
            //    securityQuestion.ModifiedByBehalfOf
            //     )
            // ).ToArray());
        }
          else
        {
            return new Response<List<GetSecurityQuestionResponse>>
            {
                Data = null,
                Result = new Result(Status.Success, "No content")
            };
        }
    }
    static IResponse<GetSecurityQuestionResponse> postSecurityQuestion(
           [FromBody] PostSecurityQuestionRequest data,
           [FromServices] UserDBContext context
           )
    {
        var securityQuestion = context!.SecurityQuestions!
          .FirstOrDefault(x => x.Question == data.Question);
        if (securityQuestion == null)
        {
             var newRecord = ObjectMapper.Mapper.Map<SecurityQuestion>(data);
            newRecord.CreatedAt = DateTime.UtcNow;
            // var newRecord = new SecurityQuestion
            // {
            //     Id = Guid.NewGuid(),
            //     Question = data.Question,
            //     CreatedAt = DateTime.Now,
            //     CreatedBy = data.CreatedBy,
            //     CreatedByBehalfOf = data.CreatedByBehalfOf
            // };
            context!.SecurityQuestions!.Add(newRecord);
            context.SaveChanges();
             return new Response<GetSecurityQuestionResponse>
            {
                Data = ObjectMapper.Mapper.Map<GetSecurityQuestionResponse>(newRecord),
                Result = new Result(Status.Success, "Add successfull")
            };
        }
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.Question != null && data.Question != securityQuestion.Question) { securityQuestion.Question = data.Question; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != securityQuestion.ModifiedBy) { securityQuestion.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != securityQuestion.ModifiedByBehalfOf) { securityQuestion.ModifiedByBehalfOf = data.ModifiedByBehalof; hasChanges = true; }
            securityQuestion.ModifiedAt = DateTime.Now;
            if (hasChanges)
            {
                context!.SaveChanges();
                return new Response<GetSecurityQuestionResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetSecurityQuestionResponse>(securityQuestion),
                    Result = new Result(Status.Success, "Update successfull")
                };
            }
            else
            {
                return new Response<GetSecurityQuestionResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetSecurityQuestionResponse>(securityQuestion),
                    Result = new Result(Status.Error, "Not modified")
                };
            }

        }
        return new Response<GetSecurityQuestionResponse>
        {
            Data = ObjectMapper.Mapper.Map<GetSecurityQuestionResponse>(securityQuestion),
            Result = new Result(Status.Error, "Request is already used for another record")
        };
    }
    static IResponse deleteSecurityQuestion(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.SecurityQuestions?.FirstOrDefault(t => t.Id == id);

       if (recordToDelete == null)
        {
            return new NoDataResponse
            {
                Result = new Result(Status.Error, "Not found question")
            };
        }
        else
        {
            context!.Remove(recordToDelete);
            context.SaveChanges();
             return new NoDataResponse
            {
                Result = new Result(Status.Success, "Delete successful")
            };
        }
    }
}

