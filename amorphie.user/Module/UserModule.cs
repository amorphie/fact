

using System.ComponentModel.DataAnnotations;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
using amorphie.user;
using amorphie.user.data;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public static class UserModule
{
    const string STATE_STORE = "amorphie-cache";
    static WebApplication _app = default!;

    public static void MapUserEndpoints(this WebApplication app)
    {
        _app = app;

    //     _app.MapGet("/user", getAllUsers)
    //    .WithOpenApi()
    //    .WithSummary("Gets registered users.")
    //    .WithDescription("Returns existing users with metadata.Query parameter reference is can contain request or order reference of user.")
    //    .WithTags("User")
    //    .Produces<GetUserResponse>(StatusCodes.Status200OK)
    //    .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/user/search", getAllUserWithFullTextSearch)
       .WithOpenApi()
       .WithSummary("Gets registered users.")
       .WithDescription("Returns existing users with metadata.Query parameter reference is can contain request or order reference of user.")
       .WithTags("User")
       .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/user/phone/countrycode/{countrycode}/prefix/{prefix}/number/{number}", getPhoneUser)
        .WithOpenApi()
        .WithSummary("Returns phone users records.")
        .WithDescription("Returns phone users records")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/user/email/{email}/", getEmailUser)
      .WithOpenApi()
      .WithSummary("Returns email users records.")
      .WithDescription("Returns email users records")
      .WithTags("User")
      .Produces<GetUserResponse>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

        _app.MapPost("/user", postUser)
        .WithOpenApi()
        .WithSummary("Save user")
        .WithDescription("It is update or creates new user.")
        .WithTags("User")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        _app.MapDelete("/user/{id}", deleteUser)
        .WithOpenApi()
        .WithSummary("Deletes user")
        .WithDescription("Delete user.")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        _app.MapGet("/user/userId/{userId}/password/{password}", checkUserPassword)
        .WithOpenApi()
        .WithSummary("Check user password.")
        .WithDescription("Check user password")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        _app.MapPost("/user/{userId}/updatePassword", updateUserPassword)
        .WithOpenApi()
        .WithSummary("Update user password.")
        .WithDescription("Update user password.")
        .WithTags("User")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        _app.MapPost("/user/{userId}/updateEmail", updateUserEmail)
       .WithOpenApi()
       .WithSummary("Update user mail.")
       .WithDescription("Update user mail.")
       .WithTags("User")
       .Produces(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status201Created)
       .Produces(StatusCodes.Status409Conflict);

        _app.MapPost("/user/{userId}/updatePhone", updateUserPhone)
      .WithOpenApi()
      .WithSummary("Update user phone.")
      .WithDescription("Update user phone.")
      .WithTags("User")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status201Created)
      .Produces(StatusCodes.Status409Conflict);

        _app.MapPost("/user/login", login)
      .WithOpenApi()
      .WithSummary("user login with reference and password")
      .WithDescription("User login")
      .WithTags("User")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status201Created);


    }

    // static async Task<IResponse<List<GetUserResponse>>> getAllUsers(
    //   [FromServices] UserDBContext context,
    //   [FromQuery] string? Reference,
    //   HttpContext httpContext,
    //   [FromServices] DaprClient client,
    //   [FromQuery][Range(0, 100)] int page = 0,
    //   [FromQuery][Range(5, 100)] int pageSize = 100
    //   )
    // {
    //     var cacheData = await client.GetStateAsync<List<GetUserResponse>>(STATE_STORE, "GetAllUsers");
    //     if (cacheData != null)
    //     {
    //         httpContext.Response.Headers.Add("X-Cache", "Hit");
    //         return new Response<List<GetUserResponse>>
    //         {
    //             Data = cacheData,
    //             Result = new Result(Status.Success, "Getirme başarılı")
    //         };
    //         // return Results.Ok(cacheData);
    //     }
    //     var query = context!.Users!
    //         .Include(d => d.UserTags)
    //         .Include(x => x.UserPasswords)
    //         .Skip(page * pageSize)
    //         .Take(pageSize);

    //     if (!string.IsNullOrEmpty(Reference))
    //     {
    //         query = query.Where(t => t.Reference == Reference);
    //     }

    //     var users = query.ToList();


    //     if (users.Count() > 0)
    //     {
    //         var response = query.Select(x => ObjectMapper.Mapper.Map<GetUserResponse>(x)).ToList();

    //         var metadata = new Dictionary<string, string> { { "ttlInSeconds", "15" } };
    //         await client.SaveStateAsync(STATE_STORE, "GetAllUsers", response, metadata: metadata);
    //         httpContext.Response.Headers.Add("X-Cache", "Miss");
    //         return new Response<List<GetUserResponse>>
    //         {
    //             Data = response,
    //             Result = new Result(Status.Success, "Getirme başarılı")
    //         };
    //     }
    //     else
    //     {
    //         return new Response<List<GetUserResponse>>
    //         {
    //             Data = null,
    //             Result = new Result(Status.Error, "the user was not found")
    //         };
    //     }
    // }
    static async Task<IResponse<List<GetUserResponse>>> getAllUserWithFullTextSearch(
        [FromServices] UserDBContext context,
        [FromQuery] string SearchText,
        HttpContext httpContext,
        [FromQuery][Range(0, 100)] int page = 0,
        [FromQuery][Range(5, 100)] int pageSize = 100
        )
    {

        var query = context!.Users!
            .Include(d => d.UserTags)
            .Include(x => x.UserPasswords)
            .Skip(page * pageSize)
            .Take(pageSize);

        if (!string.IsNullOrEmpty(SearchText))
        {
            //1.yöntem 
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.PlainToTsQuery("english", SearchText)));
            //2.yöntem
            //    query = query.Where(x => EF.Functions.ToTsVector("english",string.Join(" ",x.Reference,x.EMail,x.FirstName,x.LastName,x.State))
            //            .Matches(EF.Functions.PlainToTsQuery("english", SearchText)));
        }

        var users = query.ToList();


        if (users.Count() > 0)
        {
            var response = query.Select(x => ObjectMapper.Mapper.Map<GetUserResponse>(x)).ToList();

            return new Response<List<GetUserResponse>>
            {
                Data = response,
                Result = new Result(Status.Success, "Getirme başarılı")
            };
        }
        else
        {
            return new Response<List<GetUserResponse>>
            {
                Data = null,
                Result = new Result(Status.Error, "User was not found")
            };
        }
    }

    static IResponse<List<GetUserResponse>> getPhoneUser(
    [FromRoute(Name = "countrycode")] int countryCode,
    [FromRoute(Name = "prefix")] int prefix,
    [FromRoute(Name = "number")] string number,
    [FromServices] UserDBContext context,
    [FromQuery][Range(0, 100)] int page = 0,
    [FromQuery][Range(5, 100)] int pageSize = 100
    )
    {
        var query = context!.Users!
            .Include(d => d.UserTags)
            .Skip(page * pageSize)
            .Take(pageSize);

        var users = query.Where(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number).ToList();

        if (users.Count() > 0)
        {
            var response = query.Select(x => ObjectMapper.Mapper.Map<GetUserResponse>(x)).ToList();
            return new Response<List<GetUserResponse>>
            {
                Data = response,
                Result = new Result(Status.Success, "List return successfull")
            };

        }
        else
        {
            return new Response<List<GetUserResponse>>
            {
                Data = null,
                Result = new Result(Status.Error, "User is not found")
            };
        }
    }
    static IResponse<List<GetUserResponse>> getEmailUser(
   [FromRoute(Name = "email")] string email,
   [FromServices] UserDBContext context,
   [FromQuery][Range(0, 100)] int page = 0,
   [FromQuery][Range(5, 100)] int pageSize = 100
   )
    {
        var query = context!.Users!
            .Include(d => d.UserTags)
            .Skip(page * pageSize)
            .Take(pageSize);
        if (!String.IsNullOrEmpty(email))
        {
            var users = query.Where(t => t.EMail == email).ToList();

            if (users.Count() > 0)
            {
                var response = query.Select(x => ObjectMapper.Mapper.Map<GetUserResponse>(x)).ToList();
                return new Response<List<GetUserResponse>>
                {
                    Data = response,
                    Result = new Result(Status.Success, "Getirme başarılı")
                };
            }
            else
            {
                return new Response<List<GetUserResponse>>
                {
                    Data = null,
                    Result = new Result(Status.Error, "User is not found")
                };
            }
        }
        else
        {
            return new Response<List<GetUserResponse>>
            {
                Data = null,
                Result = new Result(Status.Error, "Email is null")
            };

        }
    }
    static async Task<IResponse<GetUserResponse>> postUser(
           [FromBody] PostUserRequest data,
           [FromServices] UserDBContext context,
             IConfiguration configuration
           )

    {

        using var transaction = context.Database.BeginTransaction();

        // Check any Reference is exists ?
        var user = context!.Users!
          .FirstOrDefault(x => x.Reference == data.Reference);


        if (user == null)
        {
            try
            {
                var salt = PasswordHelper.CreateSalt();
                var password = PasswordHelper.HashPassword(data.Password, salt);
                var result = Convert.ToBase64String(password);
                var record = ObjectMapper.Mapper.Map<User>(data);
                record.CreatedAt = DateTime.UtcNow;
                record.State = "new";
                record.Salt = Convert.ToBase64String(salt);

                context!.Users!.Add(record);
                context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = result, CreatedBy = data.CreatedBy, CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, UserId = record.Id });

                context.SaveChanges();
                transaction.Commit();
                return new Response<GetUserResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserResponse>(record),
                    Result = new Result(Status.Success, "Add successfull")
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new Response<GetUserResponse>
                {
                    Data = null,
                    Result = new Result(Status.Error, ex.ToString())
                };

                // Other steps for handling failures
            }
        }
        else
        {
            try
            {
                var hasChanges = false;
                // Apply update to only changed fields.
                if (data.FirstName != null && data.FirstName != user.FirstName) { user.FirstName = data.FirstName; hasChanges = true; }
                if (data.LastName != null && data.LastName != user.LastName) { user.LastName = data.LastName; hasChanges = true; }
                if (data.Password != null)
                {

                    var passwordSalt = Convert.FromBase64String(user.Salt);
                    var password = PasswordHelper.HashPassword(data.Password, passwordSalt);
                    var resultPassword = Convert.ToBase64String(password);

                    context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = resultPassword, CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, UserId = user.Id, ModifiedBy = data.ModifiedBy, ModifiedAt = DateTime.UtcNow });

                }

                if (data.Reference != null && data.Reference != user.Reference) { user.Reference = data.Reference; hasChanges = true; }
                if (data.State != null && data.State != user.State) { user.State = data.State; hasChanges = true; }
                if (data.EMail != null && data.EMail != user.EMail) { user.EMail = data.EMail; hasChanges = true; }
                if (data.Phone != null && data.Phone != user.Phone) { user.Phone = data.Phone; hasChanges = true; }
                if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != user.ModifiedByBehalfOf) { user.ModifiedByBehalfOf = data.ModifiedByBehalof; hasChanges = true; }
                if (data.ModifiedBy != null && data.ModifiedBy != user.ModifiedBy) { user.ModifiedBy = data.ModifiedBy; hasChanges = true; }
                user.ModifiedAt = DateTime.Now;
                if (hasChanges)
                {
                    context!.SaveChanges();
                    transaction.Commit();
                    return new Response<GetUserResponse>
                    {
                        Data = ObjectMapper.Mapper.Map<GetUserResponse>(user),
                        Result = new Result(Status.Success, "Add successfull")
                    };
                }
                else
                {
                    return new Response<GetUserResponse>
                    {
                        Data = ObjectMapper.Mapper.Map<GetUserResponse>(user),
                        Result = new Result(Status.Error, "Not Modified")
                    };

                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new Response<GetUserResponse>
                {
                    Data = null,
                    Result = new Result(Status.Error, ex.ToString())
                };

               
            }
        }


        return new Response<GetUserResponse>
        {
            Data = ObjectMapper.Mapper.Map<GetUserResponse>(user),
            Result = new Result(Status.Error, "Request  is already used for another record")
        };
    }

    static IResponse deleteUser(
       [FromRoute(Name = "id")] Guid id,
       [FromServices] UserDBContext context)
    {

        var existingRecord = context?.Users?.FirstOrDefault(t => t.Id == id);

        if (existingRecord == null)
        {
            return new NoDataResponse
            {
                Result = new Result(Status.Error, "Not found user")
            };
        }
        else
        {
            context!.Remove(existingRecord);
            context.SaveChanges();
            return new NoDataResponse
            {
                Result = new Result(Status.Error, "Delete successful")
            };
        }
    }
    static IResponse<GetUserResponse> updateUserPassword(
        [FromRoute(Name = "userId")] Guid userId,
        [FromBody] UserPasswordUpdateRequest request,
        [FromServices] UserDBContext context
          )
    {
        var user = context!.Users!
        .Include(x => x.UserPasswords)
        .FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            var userPassword = user.UserPasswords.OrderByDescending(o => o.CreatedAt).Select(s => s.HashedPassword).FirstOrDefault();
            var bytePassword = Convert.FromBase64String(userPassword);
            var salt = Convert.FromBase64String(user.Salt);
            var checkPassword = PasswordHelper.VerifyHash(request.oldPassword, salt, bytePassword);
            if (checkPassword)
            {

                var password = PasswordHelper.HashPassword(request.newPassord, salt);
                context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = Convert.ToBase64String(password), CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, UserId = user.Id, ModifiedBy = userId, ModifiedAt = DateTime.UtcNow });
                context!.SaveChanges();
                return new Response<GetUserResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserResponse>(user),
                    Result = new Result(Status.Success, "Change password")
                };

            }
            else
            {
                return new Response<GetUserResponse>
                {
                    Data = ObjectMapper.Mapper.Map<GetUserResponse>(user),
                    Result = new Result(Status.Error, "Old Passwords do not match")
                };

            }

        }
        else
        {
            return new Response<GetUserResponse>
            {
                Data = ObjectMapper.Mapper.Map<GetUserResponse>(user),
                Result = new Result(Status.Error, "User is not found")
            };

        }
    }
    static IResponse checkUserPassword(
    [FromBody] UserCheckPasswordRequest checkPasswordRequest,
    [FromServices] UserDBContext context
    )
    {

        var user = context!.Users!
        .Include(x => x.UserPasswords)
        .FirstOrDefault(x => x.Id == checkPasswordRequest.UserId);
        if (user != null)
        {
            if (user.UserPasswords != null && user.UserPasswords.Count() > 0)
            {
                var userPassword = user.UserPasswords.Where(x => x.UserId == user.Id).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if (userPassword != null)
                {
                    var bytePassword = Convert.FromBase64String(userPassword.HashedPassword);
                    var salt = Convert.FromBase64String(user.Salt);
                    var checkPassword = PasswordHelper.VerifyHash(checkPasswordRequest.Password, salt, bytePassword);
                    if (checkPassword)
                    {
                        return new NoDataResponse
                        {
                            Result = new Result(Status.Success, "Password match")
                        };
                    }

                    else
                    {
                        return new NoDataResponse
                        {

                            Result = new Result(Status.Error, "Passwords do not match")
                        };

                    }
                }
                else
                {
                    return new NoDataResponse
                    {

                        Result = new Result(Status.Error, "User password is null")
                    };
                }
            }
            else
            {
                return new NoDataResponse
                {

                    Result = new Result(Status.Error, "User password is null")
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
    static IResponse updateUserEmail(
          [FromRoute(Name = "userId")] Guid userId,
          [FromQuery] string newEmail,
          [FromServices] UserDBContext context
            )
    {

        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {

            user.EMail = newEmail;
            context!.SaveChanges();
            return new NoDataResponse
            {
                Result = new Result(Status.Success, "Change email")
            };

        }
        else
        {
            return new NoDataResponse
            {

                Result = new Result(Status.Error, "User is not found")
            };
        }
    }
    static IResponse updateUserPhone(
              [FromRoute(Name = "userId")] Guid userId,
              [FromQuery] int countryCode,
              [FromQuery] int prefix,
              [FromQuery] string number,
              [FromServices] UserDBContext context
     )
    {

        var user = context!.Users!.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {

            user.Phone.CountryCode = countryCode;
            user.Phone.Prefix = prefix;
            user.Phone.Number = number;
            context!.SaveChanges();
            return new NoDataResponse
            {
                Result = new Result(Status.Success, "Change phone")
            };
        }
        else
        {
            return new NoDataResponse
            {

                Result = new Result(Status.Error, "User is not found")
            };
        }
    }

    static IResponse login(

             [FromBody] UserLoginRequest loginRequest,
             [FromServices] UserDBContext context
    )
    {

        var user = context!.Users!.FirstOrDefault(x => x.Reference == loginRequest.Reference);
        if (user != null)
        {
            var passwordRequest = new UserCheckPasswordRequest(loginRequest.Password, user.Id);

            var responsePassword = checkUserPassword(passwordRequest, context);

            if (responsePassword.Result.Status == Status.Success.ToString())
            {

                return new NoDataResponse
                {
                    Result = new Result(Status.Success, "Login is successful")
                };
            }
            else
            {

                return new NoDataResponse
                {
                    Result = new Result(Status.Error, "Invalid reference or password")
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


