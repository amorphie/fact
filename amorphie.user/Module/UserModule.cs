using System.ComponentModel.DataAnnotations;
//using System.Text.Json;
using Newtonsoft.Json;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
using amorphie.core.Module.minimal_api;
using amorphie.fact.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dapr.Client;

public class UserModule : BaseRoute
{

    private DaprClient _daprClient;
    private IConfiguration _configuration;
    public UserModule(WebApplication app,DaprClient daprClient,IConfiguration configuration) : base(app)
    {
        _daprClient = daprClient;
        _configuration = configuration;
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
       .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("/phone/countrycode/{countrycode}/prefix/{prefix}/number/{number}", getPhoneUser)
        .WithOpenApi()
        .WithSummary("Returns phone users records.")
        .WithDescription("Returns phone users records")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("/email/{email}/", getEmailUser)
      .WithOpenApi()
      .WithSummary("Returns email users records.")
      .WithDescription("Returns email users records")
      .Produces<GetUserResponse>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapPost("/", postUser)
        .WithOpenApi()
        .WithSummary("Save user")
        .WithDescription("It is update or creates new user.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);
        routeGroupBuilder.MapPost("/postWorkflowStatus", postWorkflowStatus)
        .WithOpenApi()
        .WithSummary("Get Workflow Status")
        .WithDescription("It is update or creates new user.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);
          routeGroupBuilder.MapPost("/postOpenBankingStatus", postOpenBankingStatus)
        .WithOpenApi()
        .WithSummary("Get Workflow Status")
        .WithDescription("It is update or creates new user.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapDelete("/{id}", deleteUser)
        .WithOpenApi()
        .WithSummary("Deletes user")
        .WithDescription("Delete user.")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("/userId/{userId}/password/{password}", checkUserPassword)
        .WithOpenApi()
        .WithSummary("Check user password.")
        .WithDescription("Check user password")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapPost("/{userId}/updatePassword", updateUserPassword)
        .WithOpenApi()
        .WithSummary("Update user password.")
        .WithDescription("Update user password.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapPost("/{userId}/updateEmail", updateUserEmail)
       .WithOpenApi()
       .WithSummary("Update user mail.")
       .WithDescription("Update user mail.")
       .Produces(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status201Created)
       .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapPost("/{userId}/updatePhone", updateUserPhone)
      .WithOpenApi()
      .WithSummary("Update user phone.")
      .WithDescription("Update user phone.")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status201Created)
      .Produces(StatusCodes.Status409Conflict);

        routeGroupBuilder.MapPost("/login", login)
      .WithOpenApi()
      .WithSummary("user login with reference and password")
      .WithDescription("User login")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status201Created);

        routeGroupBuilder.MapGet("/{id}", Get)
       .WithOpenApi()
       .WithSummary("Gets registered user by id.")
       .WithDescription("Gets registered user by id.")
       .Produces<GetUserResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);


        routeGroupBuilder.MapGet("/", GetAll)
       .WithOpenApi()
       .WithSummary("Gets all registered users.")
       .Produces<GetUserResponse[]>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status204NoContent);
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

        if (!string.IsNullOrEmpty(userSearch.UserTag))
        {
            query = query.AsNoTracking().Where(p => p.UserTags.Any(t => t.Tag == userSearch.UserTag));
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
                var record = ObjectMapper.Mapper.Map<User>(data);
                record.CreatedAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(data.State))
                    record.State = "new";
                else
                {
                    record.State = data.State;
                }
                record.Salt = Convert.ToBase64String(salt);
                if (record.Id == null)
                {
                    record.Id = new Guid();
                }


                context!.Users!.Add(record);
                if (!string.IsNullOrEmpty(data.Password))
                {

                    var password = ArgonPasswordHelper.HashPassword(data.Password, salt);
                    var result = Convert.ToBase64String(password);
                    context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = result, CreatedBy = data.CreatedBy, CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, IsArgonHash = true, UserId = record.Id });
                }

                if (data.tags != null && data.tags.Count > 0)
                {
                    foreach (var tag in data.tags)
                    {
                        context.UserTags!.Add(new UserTag { Id = new Guid(), UserId = record.Id, Tag = tag });
                    }
                }


                context.SaveChanges();
                transaction.Commit();

                return Results.Ok();
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
                var hasStatusChanged = false;
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
                if (data.tags != null && data.tags.Count > 0)
                {
                    List<UserTag> userTagList = context.UserTags.Where(w => w.UserId == user.Id).ToList();
                    if (userTagList == null || (userTagList != null && userTagList.Count == 0))
                    {
                        foreach (var tag in data.tags)
                        {
                            context.UserTags.Add(new UserTag()
                            {
                                Tag = tag,
                                UserId = user.Id
                            });
                            hasChanges = true;
                        }
                    }
                    else
                    {
                        foreach (var tag in data.tags)
                        {
                            //User Tag if not exist add
                            if (!userTagList.Any(w => w.Tag == tag))
                            {
                                context.UserTags.Add(new UserTag()
                                {
                                    Tag = tag,
                                    UserId = user.Id
                                });
                                hasChanges = true;
                            }

                        }
                        //
                        foreach (var tag in userTagList)
                        {
                            //User Tag delete
                            if (!data.tags.Any(w => w == tag.Tag))
                            {
                                context.UserTags.Remove(tag);
                                hasChanges = true;
                            }

                        }
                    }


                }
                if (data.Reference != null && data.Reference != user.Reference) { user.Reference = data.Reference; hasChanges = true; }
                if (data.State != null && data.State != user.State) { user.State = data.State; hasChanges = true; hasStatusChanged = true;}
                if (data.EMail != null && data.EMail != user.EMail) { user.EMail = data.EMail; hasChanges = true; }
                if (data.Phone != null && data.Phone != user.Phone) { user.Phone = data.Phone; hasChanges = true; }
                if (data.ModifiedByBehalof != null && data.ModifiedByBehalof != user.ModifiedByBehalfOf) { user.ModifiedByBehalfOf = data.ModifiedByBehalof; hasChanges = true; }
                if (data.ModifiedBy != null && data.ModifiedBy != user.ModifiedBy) { user.ModifiedBy = data.ModifiedBy; hasChanges = true; }
                user.ModifiedAt = DateTime.Now;
                if (hasChanges)
                {
                    context!.SaveChanges();
                    transaction.Commit();
                    if(data.State != null)
                    {
                        if(hasStatusChanged && (data.State.ToLower().Equals("deactive") || data.State.ToLower().Equals("suspend")))
                        {
                            await _daprClient.InvokeMethodAsync(HttpMethod.Put,_configuration["TokenServiceAppName"],_configuration["TokenServiceRevokeMethod"]+user.Reference);
                        }
                    }
                    return Results.Ok();
                }
                else
                {
                    return Results.NoContent();
                }

            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return Results.Problem(ex.ToString());
            }
        }
    }
    async Task<IResult> postWorkflowStatus(
            [FromBody] PostWorkflow? workflowData,
            [FromServices] UserDBContext context,
              IConfiguration configuration
            )

    {
        if (workflowData != null && workflowData.workflowName == "user")
        {
            var serializeEntityData = System.Text.Json.JsonSerializer.Serialize(workflowData.entityData);
            var serializeWorkflowData = System.Text.Json.JsonSerializer.Serialize(workflowData);
            PostUserRequest requestEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<PostUserRequest>(serializeEntityData)!;
            PostWorkflowDtoUser request = Newtonsoft.Json.JsonConvert.DeserializeObject<PostWorkflowDtoUser>(serializeWorkflowData)!;
            request.data = requestEntity;
            var userWithId = context!.Users!
         .FirstOrDefault(x => x.Id == workflowData!.recordId);
            try
            {
                // Check any ID is exists ?
                if (userWithId == null)
                {
                    var userWithReferenceId = context!.Users!
                             .FirstOrDefault(x => x.Reference == requestEntity.Reference);
                    if (userWithReferenceId != null)
                    {
                        return Results.Problem("Reference already exist but workflow recordid is not:" + workflowData.recordId);
                    }
                }
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
            return postUser(ObjectMapper.Mapper.Map<PostUserRequest>(request), context, configuration).Result;
        }
        else if (workflowData != null && workflowData.workflowName == "user-reset-password")
        {
            var serializeWorkflowData = System.Text.Json.JsonSerializer.Serialize(workflowData.entityData);
            var requestEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<PostWorkflowUserReset>(serializeWorkflowData)!;

            return resetUserPassword(workflowData.recordId, requestEntity, context).Result;
        }
        return Results.Ok();
    }
    async Task<IResult> postOpenBankingStatus(
                [FromBody] PostWorkflow? workflowData,
                [FromServices] UserDBContext context,
                  IConfiguration configuration
                )

    {
        if (workflowData != null && workflowData.workflowName == "OpenBanking-Register")
        {
            User? user;
            bool hasChanges=false;
            var serializeWorkflowData = System.Text.Json.JsonSerializer.Serialize(workflowData.entityData);
            var requestEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<OpenBankingUser>(serializeWorkflowData)!;
            if (workflowData.newStatus == "openbanking-register-resend-sms" || workflowData.newStatus == "openbanking-register-send-sms")
            {
                user = new User()
                {
                    Id = workflowData.recordId,
                    State = "DeActive",
                    Reference = requestEntity.reference,
                    Phone = requestEntity.phone
                };
                context!.Users!.Add(user);
                hasChanges=true;
            }
            else
            {
                user = context.Users!.FirstOrDefault(f => f.Id == workflowData.recordId);
            }
            if (workflowData.newStatus == "openbanking-personel-password-waiting")
            {
                 
                user!.FirstName = requestEntity.firstName;
                user.LastName = requestEntity.lastName;
                user.EMail = requestEntity.eMail;
                 hasChanges=true;
            }
            if (workflowData.newStatus == "openbanking-security-question-waiting")
            {
                    var passwordSalt = Convert.FromBase64String(user!.Salt);
                var password = ArgonPasswordHelper.HashPassword(requestEntity.password, passwordSalt);
                var resultPassword = Convert.ToBase64String(password);

                context.UserPasswords!.Add(new UserPassword { Id = new Guid(), HashedPassword = resultPassword, CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, IsArgonHash = true, UserId = user.Id, ModifiedBy = workflowData.user.GetValueOrDefault(), ModifiedAt = DateTime.UtcNow });
                hasChanges=true;

                
                
            }
            if (workflowData.newStatus == "openbanking-security-image-waiting")
            {
                UserSecurityQuestion question=new UserSecurityQuestion();
                question.UserId=user!.Id;
                question.SecurityQuestionId=requestEntity.question;
                question.SecurityAnswer=requestEntity.answer;
                question.Id=new Guid();
                question.CreatedAt = DateTime.UtcNow;
                question.CreatedBy = workflowData.user.GetValueOrDefault();
                context.UserSecurityQuestions!.Add(question);
                 hasChanges=true;
            }
            if (workflowData.newStatus == "openbanking-conract1-confirm-waiting")
            {
UserSecurityImage image=new UserSecurityImage();
                image.UserId=user!.Id;
                image.SecurityImage=requestEntity.imageId.ToString();
                image.Id=new Guid();
                image.CreatedAt = DateTime.UtcNow;
                image.CreatedBy = workflowData.user.GetValueOrDefault();
                context.UserSecurityImages!.Add(image);
                hasChanges=true;


            }
            if (workflowData.newStatus == "openbanking-conract2-confirm-waiting")
            {

            }
            if (workflowData.newStatus == "user-active")
            {
                    user!.State="Active";
                    hasChanges=true;
            }
                if(hasChanges)
                context.SaveChanges();

            
        }
        return Results.Ok();
    }

    async ValueTask<IResult> resetUserPassword(
         Guid userId,
         PostWorkflowUserReset request,
         UserDBContext context
          )
    {
        var user = await context!.Users!
        .Include(x => x.UserPasswords)
        .FirstOrDefaultAsync(x => x.Id == userId);

        if (user != null)
        {
            var salt = Convert.FromBase64String(user.Salt);
            var password = ArgonPasswordHelper.HashPassword(request.NewPassword, salt);

            context.UserPasswords.Add(new UserPassword { Id = new Guid(), HashedPassword = Convert.ToBase64String(password), CreatedAt = DateTime.UtcNow, MustResetPassword = true, AccessFailedCount = 0, IsArgonHash = true, UserId = user.Id, ModifiedBy = userId, ModifiedAt = DateTime.UtcNow });
            context!.SaveChanges();

            return Results.Ok("Change password");
        }

        return Results.Problem("User is not found");
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

    async ValueTask<IResponse> checkUserPassword(
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
                        return new NoDataResponse
                        {
                            Result = new Result(Status.Success, "Password match")
                        };
                    }

                    return new NoDataResponse
                    {
                        Result = new Result(Status.Error, "Passwords do not match")
                    };
                }

                return new NoDataResponse
                {
                    Result = new Result(Status.Error, "User password is null")
                };
            }
            return new NoDataResponse
            {
                Result = new Result(Status.Error, "User password is null")
            };
        }

        return new NoDataResponse
        {
            Result = new Result(Status.Error, "User is not found")
        };
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

                var responsePassword = await checkUserPassword(passwordRequest, context);

                if (responsePassword.Result.Status == Status.Success.ToString())
                {

                    return Results.Ok(
                        new
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Reference = user.Reference,
                            EMail = user.EMail,
                            State = user.State,
                            Id = user.Id
                        }
                        );

                }

                return Results.Problem(detail:"Invalid Reference or Password",title:"Flow Exception",statusCode:461);
            }
            else
            {
                var passwordRequest = new UserCheckPasswordRequest(loginRequest.Password, user.Id);

                var responsePassword = await checkUserPbkdfPassword(passwordRequest, context);

                if (responsePassword.Result.Status == Status.Success.ToString())
                {

                    return Results.Ok(
                        new
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Reference = user.Reference,
                            EMail = user.EMail,
                            State = user.State,
                            Id = user.Id
                        });

                }

                return Results.Problem(detail:"Invalid Reference or Password",title:"Flow Exception",statusCode:461);

            }
        }
        else
        {
            return Results.Problem(detail:"User Not Found",title:"Flow Exception",statusCode:460);
        }
    }

    async ValueTask<IResult> Get(
     [FromServices] UserDBContext context,
     [FromRoute(Name = "id")] Guid id)
    {
        var user = context!.Users!
           .Include(d => d.UserTags)
           .Include(x => x.UserPasswords)
           .FirstOrDefault(t => t.Id == id);

        if (user is User)
        {
            return TypedResults.Ok(ObjectMapper.Mapper.Map<GetUserResponse>(user));
        }

        return TypedResults.NotFound();
    }

    async ValueTask<IResult> GetAll([FromServices] UserDBContext context,
            [FromQuery][Range(0, 100)] int page,
            [FromQuery][Range(5, 100)] int pageSize)
    {
        var resultList = await context!.Users!
           .Include(d => d.UserTags)
           .Include(x => x.UserPasswords)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (resultList != null && resultList.Count() > 0)
        {
            var response = resultList.Select(x => ObjectMapper.Mapper.Map<GetUserResponse>(x)).ToList();

            return Results.Ok(response);
        }

        return Results.NoContent();
    }

}