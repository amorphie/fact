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

public static class UserTagModule
{

    static WebApplication _app = default!;

    public static void MapUserTagEndpoints(this WebApplication app)
    {
        _app = app;

    //     _app.MapGet("/usertag", getAllUserTag)
    //    .WithTags("UserTag")
    //    .WithOpenApi(operation =>
    //    {
    //        operation.Summary = "Returns saved usertag records.";
    //        operation.Parameters[0].Description = "Filtering parameter. Given **usertag** is used to filter users.";
    //        operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
    //        operation.Parameters[2].Description = "Paging parameter. **Token** is returned from last query.";
    //        return operation;
    //    })
    //    .Produces<GetUserTagResponse[]>(StatusCodes.Status200OK)
    //    .Produces(StatusCodes.Status204NoContent);

          _app.MapGet("/usertag", getAllUserTagFullTextSearch)
       .WithTags("UserTag")
       .WithOpenApi(operation =>
       {
           operation.Summary = "Returns saved usertag records.";
           operation.Parameters[0].Description = "Filtering parameter. Given **usertag** is used to filter users.";
           operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
           operation.Parameters[2].Description = "Paging parameter. **Token** is returned from last query.";
           return operation;
       })
       .Produces<GetUserTagResponse[]>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status204NoContent);

        _app.MapPost("/usertag", postUserTag)
         .WithOpenApi()
         .WithSummary("Save usertag")
         .WithDescription("")
         .WithTags("UserTag")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);



        _app.MapDelete("/usertag/{id}", deleteUserTag)
        .WithOpenApi()
        .WithSummary("Deletes usertag")
        .WithDescription("Delete usertag.")
        .WithTags("UserTag")
        .Produces<GetUserTagResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);


    }
    // static IResponse<List<GetUserTagResponse>> getAllUserTag(
    //   [FromServices] UserDBContext context,
    //   [FromQuery] string? Tag,
    //   [FromQuery][Range(0, 100)] int page = 0,
    //   [FromQuery][Range(5, 100)] int pageSize = 100
    //   )
    // {
    //     var query = context!.UserTags!
    //         .Skip(page * pageSize)
    //         .Take(pageSize);

    //     if (!string.IsNullOrEmpty(Tag))
    //     {
    //         query = query.Where(x => x.Tag.Contains(Tag));
    //     }

    //     var userTags = query.ToList();

    //     if (userTags.Count() > 0)
    //     {
    //         return new Response<List<GetUserTagResponse>>
    //         {
    //             Data = userTags.Select(x => ObjectMapper.Mapper.Map<GetUserTagResponse>(x)).ToList(),
    //             Result = new Result(Status.Success, "List return successfull")
    //         };
    //     }
    //     else
    //     {
    //         return new Response<List<GetUserTagResponse>>
    //         {
    //             Data = null,
    //             Result = new Result(Status.Success, "No content")
    //         };
    //     }
    // }
     static IResponse<List<GetUserTagResponse>> getAllUserTagFullTextSearch(
      [FromServices] UserDBContext context,
      [FromQuery] string? SearchText,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var query = context!.UserTags!
            .Skip(page * pageSize)
            .Take(pageSize);

        if (!string.IsNullOrEmpty(SearchText))
        {
            query = query.Where(x => EF.Functions.ToTsVector("english",string.Join(" ",x.Tag,x.UserId,x.Id))
           .Matches(EF.Functions.PlainToTsQuery("english", SearchText)));
  
        }

        var userTags = query.ToList();

        if (userTags.Count() > 0)
        {
            return new Response<List<GetUserTagResponse>>
            {
                Data = userTags.Select(x => ObjectMapper.Mapper.Map<GetUserTagResponse>(x)).ToList(),
                Result = new Result(Status.Success, "List return successfull")
            };
        }
        else
        {
            return new Response<List<GetUserTagResponse>>
            {
                Data = null,
                Result = new Result(Status.Success, "No content")
            };
        }
    }
    static IResponse<GetUserTagResponse> postUserTag(
           [FromBody] PostUserTagRequest data,
           [FromServices] UserDBContext context
           )
    {

        var userTag = context!.UserTags!
          .FirstOrDefault(x => x.Tag == data.Tag && x.UserId == data.UserId);


        if (userTag == null)
        {
            var newRecord = ObjectMapper.Mapper.Map<UserTag>(data);
            newRecord.CreatedAt = DateTime.UtcNow;
            context!.UserTags!.Add(newRecord);
            context.SaveChanges();
            return new Response<GetUserTagResponse>
            {
                Data = ObjectMapper.Mapper.Map<GetUserTagResponse>(newRecord),
                Result = new Result(Status.Success, "Add successfull")
            };
        }
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.Tag != null && data.Tag != userTag.Tag) { userTag.Tag = data.Tag; hasChanges = true; }
            if (data.UserId != null && data.UserId != userTag.UserId) { userTag.UserId = data.UserId; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != userTag.ModifiedBy) { userTag.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != userTag.ModifiedByBehalfOf) { userTag.ModifiedByBehalfOf = data.ModifiedByBehalof; hasChanges = true; }
            userTag.ModifiedAt = DateTime.Now;
            if (hasChanges)
            {
                context!.SaveChanges();
                return new Response<GetUserTagResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserTagResponse>(userTag),
                    Result = new Result(Status.Success, "Update successfull")
                };
            }
            else
            {
                return new Response<GetUserTagResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserTagResponse>(userTag),
                    Result = new Result(Status.Error, "Not modified")
                };

            }
          
        }
         return new Response<GetUserTagResponse>
        {
             Data = ObjectMapper.Mapper.Map<GetUserTagResponse>(userTag),
            Result = new Result(Status.Error, "Request is already used for another record")
        };
    }
        static IResponse deleteUserTag(
           [FromRoute(Name = "id")] Guid id,
           [FromServices] UserDBContext context)
        {

            var recordToDelete = context?.UserTags?.FirstOrDefault(t => t.Id == id);

            if (recordToDelete == null)
            {
                return new NoDataResponse
                {
                    Result = new Result(Status.Error, "User tag is not found")
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

