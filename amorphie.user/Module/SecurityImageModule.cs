using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using amorphie.fact.data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
//using Core.Utilities.Results;
//using IResult = Core.Utilities.Results.IResult;
using Result = amorphie.core.Base.Result;
public static class SecurityImageModule
{

    static WebApplication _app = default!;

    public static void MapSecurityImageEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/securityimage", getAllSecurityImage)
       .WithTags("SecurityImage")
       .WithOpenApi(operation =>
       {
           operation.Summary = "Returns saved securityimage records.";
           operation.Parameters[0].Description = "Filtering parameter. Given **securityimage** is used to filter securityimages.";
           operation.Parameters[1].Description = "Paging parameter. **limit** is the page size of resultset.";
           return operation;
       })
       .Produces<GetSecurityImageResponse[]>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status204NoContent);

        _app.MapPost("/securityimage", postSecurityImage)
         .WithOpenApi()
         .WithSummary("Save securityimage")
         .WithDescription("")
         .WithTags("SecurityImage")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/securityimage/{id}", deleteSecurityImage)
        .WithOpenApi()
        .WithSummary("Deletes securityimage")
        .WithDescription("Delete securityimage.")
        .WithTags("SecurityImage")
        .Produces<GetSecurityImageResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
    static IResponse<List<GetSecurityImageResponse>> getAllSecurityImage(
      [FromServices] UserDBContext context,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var securityImages = context!.SecurityImages!
            .Skip(page * pageSize)
            .Take(pageSize).ToList();


        if (securityImages.Count() > 0)
        {
            return new Response<List<GetSecurityImageResponse>>
            {
                Data = securityImages.Select(x => ObjectMapper.Mapper.Map<GetSecurityImageResponse>(x)).ToList(),
                Result = new Result(Status.Success, "List return successfull")
            };
        }
        else
        {
            return new Response<List<GetSecurityImageResponse>>
            {
                Data = null,
                Result = new Result(Status.Success, "No content")
            };
        }

    }
    static IResponse<GetSecurityImageResponse> postSecurityImage(
         [FromBody] PostSecurityImageRequest data,
         [FromServices] UserDBContext context
         )
    {

        var securityImage = context!.SecurityImages!
          .FirstOrDefault(x => x.Image == data.Image);


        if (securityImage == null)
        {
            var newRecord = ObjectMapper.Mapper.Map<SecurityImage>(data);
            newRecord.CreatedAt = DateTime.UtcNow;
            // var newRecord = new SecurityImage
            // {
            //     Id = Guid.NewGuid(),
            //     Image = data.Image,
            //     CreatedAt = DateTime.Now,
            //     CreatedBy = data.CreatedBy,
            //     CreatedByBehalfOf = data.CreatedByBehalfOf

            // };
            context!.SecurityImages!.Add(newRecord);
            context.SaveChanges();
            return new Response<GetSecurityImageResponse>
            {
                Data = ObjectMapper.Mapper.Map<GetSecurityImageResponse>(newRecord),
                Result = new Result(Status.Success, "Add successfull")
            };
            // return Results.Created($"/securityimage/{data.Image}", newRecord);
        }
        else
        {
            var hasChanges = false;
            // Apply update to only changed fields.
            if (data.Image != null && data.Image != securityImage.Image) { securityImage.Image = data.Image; hasChanges = true; }
            if (data.ModifiedBy != null && data.ModifiedBy != securityImage.ModifiedBy) { securityImage.ModifiedBy = data.ModifiedBy; hasChanges = true; }
            if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != securityImage.ModifiedByBehalfOf) { securityImage.ModifiedByBehalfOf = data.ModifiedByBehalof; hasChanges = true; }
            securityImage.ModifiedAt = DateTime.Now;

            if (hasChanges)
            {
                context!.SaveChanges();
                return new Response<GetSecurityImageResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetSecurityImageResponse>(securityImage),
                    Result = new Result(Status.Success, "Update successfull")
                };
                //  return Results.Ok(data);
            }
            else
            {
                return new Response<GetSecurityImageResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetSecurityImageResponse>(securityImage),
                    Result = new Result(Status.Error, "Not modified")
                };
            }

        }
        return new Response<GetSecurityImageResponse>
        {
            Data = ObjectMapper.Mapper.Map<GetSecurityImageResponse>(securityImage),
            Result = new Result(Status.Error, "Request is already used for another record")
        };
    }
    static IResponse deleteSecurityImage(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.SecurityImages?.FirstOrDefault(t => t.Id == id);

        if (recordToDelete == null)
        {
            return new NoDataResponse
            {
                Result = new Result(Status.Success, "Image is not found")
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

