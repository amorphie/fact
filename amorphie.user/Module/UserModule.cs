using System.ComponentModel.DataAnnotations;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserModule : BaseRoute
{

    public UserModule(WebApplication app) : base(app)
    {

    }

    const string STATE_STORE = "amorphie-cache";
    static WebApplication _app = default!;

    public override string? UrlFragment => "user";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        routeGroupBuilder.MapGet("/search", getAllUserWithFullTextSearch)
       .WithOpenApi()
       .WithSummary("Gets registered users.")
       .WithDescription("Returns existing users with metadata.Query parameter reference is can contain request or order reference of user.")
       .WithTags("User")
       .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("/phone/countrycode/{countrycode}/prefix/{prefix}/number/{number}", getPhoneUser)
        .WithOpenApi()
        .WithSummary("Returns phone users records.")
        .WithDescription("Returns phone users records")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("/email/{email}/", getEmailUser)
      .WithOpenApi()
      .WithSummary("Returns email users records.")
      .WithDescription("Returns email users records")
      .WithTags("User")
      .Produces<GetUserResponse>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapPost("/", postUser)
        .WithOpenApi()
        .WithSummary("Save user")
        .WithDescription("It is update or creates new user.")
        .WithTags("User")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapDelete("/{id}", deleteUser)
        .WithOpenApi()
        .WithSummary("Deletes user")
        .WithDescription("Delete user.")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("/userId/{userId}/password/{password}", checkUserPassword)
        .WithOpenApi()
        .WithSummary("Check user password.")
        .WithDescription("Check user password")
        .WithTags("User")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapPost("/{userId}/updatePassword", updateUserPassword)
        .WithOpenApi()
        .WithSummary("Update user password.")
        .WithDescription("Update user password.")
        .WithTags("User")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapPost("/{userId}/updateEmail", updateUserEmail)
       .WithOpenApi()
       .WithSummary("Update user mail.")
       .WithDescription("Update user mail.")
       .WithTags("User")
       .Produces(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status201Created)
       .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapPost("/{userId}/updatePhone", updateUserPhone)
      .WithOpenApi()
      .WithSummary("Update user phone.")
      .WithDescription("Update user phone.")
      .WithTags("User")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status201Created)
      .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapPost("/login", login)
      .WithOpenApi()
      .WithSummary("user login with reference and password")
      .WithDescription("User login")
      .WithTags("User")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status201Created);
    }

    async ValueTask<IResult> getAllUserWithFullTextSearch(
       [FromServices] UserDBContext context,
       [AsParameters] UserSearch userSearch
       )
    {
        var query = context!.Users!
            .Include(d => d.UserTags)
            .Include(x => x.UserPasswords)
            .Skip(userSearch.Page * userSearch.PageSize)
            .Take(userSearch.PageSize);

        if (!string.IsNullOrEmpty(userSearch.Keyword))
        {
            query = query.AsNoTracking().Where(p => p.SearchVector.Matches(EF.Functions.PlainToTsQuery("english", userSearch.Keyword)));
        }

        var users = query.ToList();

        if (users.Count() > 0)
        {
            var response = query.Select(x => ObjectMapper.Mapper.Map<GetUserResponse>(x)).ToList();

            return Results.Ok(response);
        }

        return Results.NoContent();
    }

    async ValueTask<IResult> getPhoneUser(
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

        var users = query.AsNoTracking().Where(t => t.Phone.CountryCode == countryCode && t.Phone.Prefix == prefix && t.Phone.Number == number).ToList();

        if (users.Count() > 0)
        {
            var response = query.Select(x => ObjectMapper.Mapper.Map<GetUserResponse>(x)).ToList();

            return Results.Ok(response);
        }

        return Results.NoContent();
    }
    async ValueTask<IResult> getEmailUser(
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
            var users = query.AsNoTracking().Where(t => t.EMail == email).ToList();

            if (users.Count() > 0)
            {
                var response = query.Select(x => ObjectMapper.Mapper.Map<GetUserResponse>(x)).ToList();

                return Results.Ok(response);
            }
        }

        return Results.NoContent();
    }
    async ValueTask<IResult> postUser(
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
                var salt = ArgonPasswordHelper.CreateSalt();
                var password = ArgonPasswordHelper.HashPassword(data.Password, salt);
                var result = Convert.ToBase64String(password);
                var record = ObjectMapper.Mapper.Map<User>(data);
                record.CreatedAt = DateTime.UtcNow;
                record.State = "new";
                record.Salt = Convert.ToBase64String(salt);

                context!.Users!.Add(record);
                context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = result, CreatedBy = data.CreatedBy, CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, IsArgonHash = true, UserId = record.Id });

                context.SaveChanges();
                transaction.Commit();

                return Results.Created($"/{record.Id}", record);
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return Results.Problem(ex.ToString());

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
                    var password = ArgonPasswordHelper.HashPassword(data.Password, passwordSalt);
                    var resultPassword = Convert.ToBase64String(password);

                    context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = resultPassword, CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, IsArgonHash = true, UserId = user.Id, ModifiedBy = data.ModifiedBy, ModifiedAt = DateTime.UtcNow });

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
                }

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return Results.Problem(ex.ToString());
            }
        }
    }

    async ValueTask<IResult> deleteUser(
      [FromRoute(Name = "id")] Guid id,
      [FromServices] UserDBContext context)
    {
        var existingRecord = await context!.Users!.FirstOrDefaultAsync(t => t.Id == id);

        if (existingRecord == null)
        {
            return Results.NotFound();
        }

        context!.Remove(existingRecord);
        context.SaveChanges();

        return Results.Ok(existingRecord);
    }
    async ValueTask<IResult> updateUserPassword(
        [FromRoute(Name = "userId")] Guid userId,
        [FromBody] UserPasswordUpdateRequest request,
        [FromServices] UserDBContext context
          )
    {
        var user = await context!.Users!
        .Include(x => x.UserPasswords)
        .FirstOrDefaultAsync(x => x.Id == userId);

        if (user != null)
        {
            var userPassword = user.UserPasswords.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            if (userPassword.IsArgonHash)
            {
                var bytePassword = Convert.FromBase64String(userPassword.HashedPassword);
                var salt = Convert.FromBase64String(user.Salt);
                var checkPassword = ArgonPasswordHelper.VerifyHash(request.oldPassword, salt, bytePassword);
                if (checkPassword)
                {

                    var password = ArgonPasswordHelper.HashPassword(request.newPassord, salt);
                    context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = Convert.ToBase64String(password), CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, IsArgonHash = true, UserId = user.Id, ModifiedBy = userId, ModifiedAt = DateTime.UtcNow });
                    context!.SaveChanges();

                    return Results.Ok("Change password");
                }

                return Results.Problem("Old Passwords do not match");
            }
            else
            {
                var checkPasswordRequest = new UserCheckPasswordRequest(request.oldPassword, user.Id);

                var pbkdfPassword = await checkUserPbkdfPassword(checkPasswordRequest, context);
                
                if (pbkdfPassword.Result.Status == Status.Success.ToString())
                {
                    var bytePassword = Convert.FromBase64String(userPassword.HashedPassword);
                    var salt = Convert.FromBase64String(user.Salt);
                    var password = ArgonPasswordHelper.HashPassword(request.newPassord, salt);
                    context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = Convert.ToBase64String(password), CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, IsArgonHash = true, UserId = user.Id, ModifiedBy = userId, ModifiedAt = DateTime.UtcNow });
                    context!.SaveChanges();

                    return Results.Ok("Change password");
                }

                return Results.Problem("Old Passwords do not match");
            }
        }

        return Results.Problem("User is not found");
    }

    async ValueTask<IResult> checkUserPassword(
   [FromBody] UserCheckPasswordRequest checkPasswordRequest,
   [FromServices] UserDBContext context
   )
    {
        var user = await context!.Users!
        .Include(x => x.UserPasswords)
        .FirstOrDefaultAsync(x => x.Id == checkPasswordRequest.UserId);

        if (user != null)
        {
            if (user.UserPasswords != null && user.UserPasswords.Count() > 0)
            {
                var userPassword = user.UserPasswords.Where(x => x.UserId == user.Id && x.IsArgonHash == true).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if (userPassword != null)
                {
                    var bytePassword = Convert.FromBase64String(userPassword.HashedPassword);
                    var salt = Convert.FromBase64String(user.Salt);
                    var checkPassword = ArgonPasswordHelper.VerifyHash(checkPasswordRequest.Password, salt, bytePassword);

                    if (checkPassword)
                    {
                        return Results.Ok("Password match");
                    }

                    return Results.Problem("Passwords do not match");
                }

                return Results.Problem("User password is null");
            }

            return Results.Problem("User password is null");
        }

        return Results.Problem("User is not found");
    }

    async ValueTask<IResponse> checkUserPbkdfPassword(
  [FromBody] UserCheckPasswordRequest checkPasswordRequest,
  [FromServices] UserDBContext context
  )
    {
        var user = await context!.Users!
        .Include(x => x.UserPasswords)
        .FirstOrDefaultAsync(x => x.Id == checkPasswordRequest.UserId);

        if (user != null)
        {
            if (user.UserPasswords != null && user.UserPasswords.Count() > 0)
            {
                var userPassword = user.UserPasswords.Where(x => x.UserId == user.Id && x.IsArgonHash == false).OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                if (userPassword != null)
                {
                    var bytePassword = Convert.FromBase64String(userPassword.HashedPassword);
                    var salt = Convert.FromBase64String(user.Salt);
                    PbkdfPasswordHelper pbkdfPasswordHelper = new PbkdfPasswordHelper();
                    PasswordVerificationResult checkPassword = pbkdfPasswordHelper.VerifyHashedPassword(userPassword.HashedPassword, checkPasswordRequest.Password, user.Salt);

                    if (checkPassword == PasswordVerificationResult.Success)
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
    async ValueTask<IResult> updateUserEmail(
         [FromRoute(Name = "userId")] Guid userId,
         [FromQuery] string newEmail,
         [FromServices] UserDBContext context
           )
    {
        var user = await context!.Users!.FirstOrDefaultAsync(x => x.Id == userId);

        if (user != null)
        {
            user.EMail = newEmail;
            context!.SaveChanges();
        }

        return Results.NoContent();
    }
    async ValueTask<IResult> updateUserPhone(
            [FromRoute(Name = "userId")] Guid userId,
            [FromQuery] int countryCode,
            [FromQuery] int prefix,
            [FromQuery] string number,
            [FromServices] UserDBContext context
   )
    {
        var user = await context!.Users!.FirstOrDefaultAsync(x => x.Id == userId);

        if (user != null)
        {
            user.Phone.CountryCode = countryCode;
            user.Phone.Prefix = prefix;
            user.Phone.Number = number;
            context!.SaveChanges();
        }

        return Results.NoContent();
    }

    async ValueTask<IResult> login(
            [FromBody] UserLoginRequest loginRequest,
            [FromServices] UserDBContext context
   )
    {
        var user = await context!.Users!
        .Include(x => x.UserPasswords)
        .FirstOrDefaultAsync(x => x.Reference == loginRequest.Reference);

        if (user != null)
        {
            var userPassword = user.UserPasswords.Where(x => x.UserId == user.Id).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            if (userPassword.IsArgonHash == true)
            {
                var passwordRequest = new UserCheckPasswordRequest(loginRequest.Password, user.Id);

                var responsePassword = checkUserPassword(passwordRequest, context);

                if (responsePassword.Result == Results.Ok("Password match"))
                {
                    return Results.Ok("Login is successful");
                }

                return Results.Problem("Invalid reference or password");
            }
            else
            {
                var passwordRequest = new UserCheckPasswordRequest(loginRequest.Password, user.Id);

                var responsePassword = await checkUserPbkdfPassword(passwordRequest, context);

                if (responsePassword.Result.Status == Status.Success.ToString())
                {
                    return Results.Ok("Login is successful");
                }

                return Results.Problem("Invalid reference or password");

            }
        }
        else
        {
            return Results.Problem("User is not found");
        }
    }

}