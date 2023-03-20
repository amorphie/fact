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

public static class UserSecurityImageModule
{

    static WebApplication _app = default!;

    public static void MapUserSecurityImageEndpoints(this WebApplication app)
    {
        _app = app;

        _app.MapGet("/usersecurityimage", getAllUserSecurityImage)
        .WithOpenApi()
       .WithSummary("Returns saved usersecurityimage records.")
       .WithDescription("Returns existing usersecurityimage with metadata.Query parameter usersecurityimage is can contain request or order SecurityQuestion of usersecurityimages.")
       .WithTags("UserSecurityImage")
       .Produces<GetUserSecurityImageResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);


        _app.MapPost("/usersecurityimage", postUserSecurityImage)
         .WithOpenApi()
         .WithSummary("Save usersecurityimage.")
         .WithDescription("Save usersecurityimage.")
         .WithTags("UserSecurityImage")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/usersecurityimage/{id}", deleteUserSecurityImage)
        .WithOpenApi()
        .WithSummary("Deletes usersecurityimage")
        .WithDescription("Delete usersecurityimage.")
        .WithTags("UserSecurityImage")
        .Produces<GetUserSecurityImageResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/usersecurityimage/user/{userId}/image/{image}", userCheckImage)
        .WithOpenApi()
       .WithSummary("Check image.")
       .WithDescription("Check image")
       .WithTags("UserSecurityImage")
       .Produces<GetUserSecurityImageResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);


    }
    static IResponse<List<GetUserSecurityImageResponse>> getAllUserSecurityImage(
      [FromServices] UserDBContext context,
      [FromQuery] Guid UserId,
      [FromQuery][Range(0, 100)] int page = 0,
      [FromQuery][Range(5, 100)] int pageSize = 100
      )
    {
        var query = context!.UserSecurityImages!

            .Skip(page * pageSize)
            .Take(pageSize);

        if (UserId != null)
        {
            query = query.Where(t => t.UserId == UserId);
        }

        var userSecurityImages = query.ToList();

        if (userSecurityImages.Count() > 0)
        {
            return new Response<List<GetUserSecurityImageResponse>>
            {
                Data = userSecurityImages.Select(x => ObjectMapper.Mapper.Map<GetUserSecurityImageResponse>(x)).ToList(),
                Result = new Result(Status.Success, "List return successfull")
            };
            //     return Results.Ok(userSecurityImages.Select(userSecurityImage =>
            //       new GetUserSecurityImageResponse(
            //        userSecurityImage.Id,
            //        userSecurityImage.SecurityImage,
            //        userSecurityImage.UserId,
            //            userSecurityImage.CreatedAt,
            //        userSecurityImage.CreatedBy,
            //    userSecurityImage.CreatedByBehalfOf,
            //    userSecurityImage.ModifiedAt,
            //        userSecurityImage.ModifiedBy,
            //        userSecurityImage.ModifiedByBehalfOf

            //         )
            //     ).ToArray());
        }
        else
        {
            return new Response<List<GetUserSecurityImageResponse>>
            {
                Data = null,
                Result = new Result(Status.Success, "No content")
            };
        }
    }
    static IResponse<GetUserSecurityImageResponse> postUserSecurityImage(
           [FromBody] PostUserSecurityImageRequest data,
           [FromServices] UserDBContext context
           )
    {

        var userSecurityImage = context!.UserSecurityImages!
          .FirstOrDefault(x => x.UserId == data.UserId);
        var user = context!.Users.FirstOrDefault(x => x.Id == data.UserId);

        if (userSecurityImage == null)
        {
            if (user.Salt != null)
            {
                var salt = Convert.FromBase64String(user.Salt);

                var password = PasswordHelper.HashPassword(data.SecurityImage, salt);
                var result = Convert.ToBase64String(password);
                var newRecord = ObjectMapper.Mapper.Map<UserSecurityImage>(data);
                newRecord.CreatedAt = DateTime.UtcNow;
                newRecord.SecurityImage = result;


                // var newRecord = new UserSecurityImage
                // {
                //     Id = Guid.NewGuid(),
                //     SecurityImage = result,
                //     UserId = data.UserId,
                //     CreatedAt = DateTime.Now,
                //     CreatedBy = data.CreatedBy,
                //     CreatedByBehalfOf = data.CreatedByBehalfOf
                // };
                context!.UserSecurityImages!.Add(newRecord);
                context.SaveChanges();
                return new Response<GetUserSecurityImageResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserSecurityImageResponse>(newRecord),
                    Result = new Result(Status.Success, "Add successfull")
                };
            }
            else
            {
                return new Response<GetUserSecurityImageResponse>
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
            if (data.SecurityImage != null)
            {
                if (user.Salt != null)
                {

                    var bytePassword = Convert.FromBase64String(userSecurityImage.SecurityImage);
                    var salt = Convert.FromBase64String(user.Salt);
                    var checkPassword = PasswordHelper.VerifyHash(data.SecurityImage, salt, bytePassword);
                    if (!checkPassword)
                    {
                        var password = PasswordHelper.HashPassword(data.SecurityImage, salt);
                        userSecurityImage.SecurityImage = Convert.ToBase64String(password);
                        hasChanges = true;
                    }
                    if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != userSecurityImage.ModifiedByBehalfOf) { userSecurityImage.ModifiedByBehalfOf = data.ModifiedByBehalof; hasChanges = true; }
                    if (data.ModifiedBy != null && data.ModifiedBy != userSecurityImage.ModifiedBy) { userSecurityImage.ModifiedBy = data.ModifiedBy; hasChanges = true; }
                    userSecurityImage.ModifiedAt = DateTime.Now;

                }

                if (hasChanges)
                {
                    context!.SaveChanges();
                    return new Response<GetUserSecurityImageResponse>
                    {
                        Data = ObjectMapper.Mapper.Map<GetUserSecurityImageResponse>(userSecurityImage),
                        Result = new Result(Status.Success, "Add successfull")
                    };

                }
                else
                {
                    return new Response<GetUserSecurityImageResponse>
                    {
                        Data = ObjectMapper.Mapper.Map<GetUserSecurityImageResponse>(data),
                        Result = new Result(Status.Error, "Not Modified")
                    };
                }
            }
            else
            {
                return new Response<GetUserSecurityImageResponse>
                {
                    Data = null,
                    Result = new Result(Status.Success, "User salt not found")
                };
            }

        }
        return new Response<GetUserSecurityImageResponse>
        {
            Data = ObjectMapper.Mapper.Map<GetUserSecurityImageResponse>(data),
            Result = new Result(Status.Error, "Request  is already used for another record")
        };
    }
    static IResponse deleteUserSecurityImage(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var recordToDelete = context?.UserSecurityImages?.FirstOrDefault(t => t.Id == id);

        if (recordToDelete == null)
        {
            return new NoDataResponse
            {
                Result = new Result(Status.Success, "Security image is not found")
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
    static  IResponse userCheckImage(
        [FromRoute(Name = "userId")] Guid userId,
        [FromRoute(Name = "image")] string image,
         [FromServices] UserDBContext context
          )
    {
        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            var securityImage = context!.UserSecurityImages!.FirstOrDefault(x => x.UserId == userId);
            if (securityImage != null)
            {
                var byteImage = Convert.FromBase64String(securityImage.SecurityImage);
                var salt = Convert.FromBase64String(user.Salt);
                var checkPassword = PasswordHelper.VerifyHash(image, salt, byteImage);
                if (checkPassword)
                {
                   return new NoDataResponse
                {
                    Result = new Result(Status.Success, "Image match")
                };
                }
                else
                {
                   return new NoDataResponse
                {

                    Result = new Result(Status.Error, "Image do not match")
                };
                }
            }
            else
            {
                  return new NoDataResponse
                {

                    Result = new Result(Status.Error, "SecurityImage definition is not found. Please check userId is exists in definitions")
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

