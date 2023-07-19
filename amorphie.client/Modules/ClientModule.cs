using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using amorphie.fact.data;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using amorphie.core.Base;
using amorphie.core.Module.minimal_api;
using amorphie.core.Identity;
using FluentValidation;
using amorphie.core.Swagger;
using Microsoft.OpenApi.Models;

namespace amorphie.client;

public class ClientModule
    : BaseBBTRoute<ClientDto, Client, UserDBContext>
{
    public ClientModule(WebApplication app) : base(app)
    {
    }

    public override string[]? PropertyCheckList => new string[] { "Type", "Validations", "Status" };

    public override string? UrlFragment => "client";


    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapPost("validate", validateClient);
        routeGroupBuilder.MapGet("search", getAllClientFullTextSearch);
        routeGroupBuilder.MapPost("workflowClient", workflowClient);
    }

    [AddSwaggerParameter("Language", ParameterLocation.Header, false)]
    protected override async ValueTask<IResult> GetMethod(
        [FromServices] UserDBContext context,
        [FromServices] IMapper mapper,
        [FromRoute(Name = "id")] Guid id,
        HttpContext httpContext,
        CancellationToken token
        )
    {
        var client = await context.Clients!.AsNoTracking()
         .Include(t => t.HeaderConfig)
         .Include(t => t.Jws)
         .Include(t => t.Idempotency)
         .Include(t => t.Names)
         .Include(t => t.Tokens)
         .Include(t => t.AllowedGrantTypes)
         .Include(t => t.Flows)
         .Include(t => t.Names.Where(t => t.Language == httpContext.GetHeaderLanguage()))
         .FirstOrDefaultAsync(t => t.Id == id, token);

        if (client is Client)
        {
            return TypedResults.Ok(ObjectMapper.Mapper.Map<ClientGetDto>(client));
        }
        else
        {
            return Results.Problem(detail:"Client Not Found",title:"Flow Exception",statusCode:460);
        }

        return TypedResults.NotFound();
    }

    [AddSwaggerParameter("Language", ParameterLocation.Header, false)]
    protected override async ValueTask<IResult> GetAllMethod(
                [FromServices] UserDBContext context,
                [FromServices] IMapper mapper,
                [FromQuery][Range(0, 100)] int page,
                [FromQuery][Range(5, 100)] int pageSize,
                HttpContext httpContext,
                CancellationToken token
                )
    {
        var resultList = await context!.Clients!.AsNoTracking()
        .Include(t => t.HeaderConfig)
         .Include(t => t.Jws)
         .Include(t => t.Idempotency)
         .Include(t => t.Names)
         .Include(t => t.Tokens)
         .Include(t => t.AllowedGrantTypes)
         .Include(t => t.Flows)
         .Include(t => t.Names.Where(t => t.Language == httpContext.GetHeaderLanguage()))
       .Skip(page * pageSize)
       .Take(pageSize)
       .ToListAsync(token);

        if (resultList != null && resultList.Count() > 0)
        {
            var response = resultList.Select(x => ObjectMapper.Mapper.Map<ClientGetDto>(x)).ToList();

            return Results.Ok(response);
        }

        return Results.NoContent();
    }

    async ValueTask<IResult> validateClient(
        [FromBody] ValidateClientRequest data,
        [FromServices] UserDBContext context,
        HttpContext httpContext,
        CancellationToken token
        )
    {
        var hashedSecret = ComputeSha256Hash(data.Secret!);

        var client = await context!.Clients!.AsNoTracking()
         .Include(t => t.HeaderConfig)
         .Include(t => t.Jws)
         .Include(t => t.Idempotency)
         .Include(t => t.Names)
         .Include(t => t.Tokens)
         .Include(t => t.AllowedGrantTypes)
         .Include(t => t.Flows)
         .Include(t => t.Names.Where(t => t.Language == httpContext.GetHeaderLanguage()))
          .FirstOrDefaultAsync(t => t.Id == data.ClientId
                && t.Secret == hashedSecret, token);

        if (client == null)
        {
            return Results.Problem(detail:"Invalid Client ID Or Client Secret",title:"Flow Exception",statusCode:461);
        }

        return Results.Ok(ObjectMapper.Mapper.Map<ClientGetDto>(client));
    }
    async ValueTask<IResult> workflowClient(
    [FromServices] IMapper mapper,
    [FromServices] ClientValidator validator,
    [FromServices] UserDBContext context,
    [FromServices] IBBTIdentity bbtIdentity,
    [FromBody] PostWorkflow workflowData,
    HttpContext httpContext,
    CancellationToken token
    )
    {
        var serializeEntityData = System.Text.Json.JsonSerializer.Serialize(workflowData.entityData);
        ClientWorkflowStatusRequest requestEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientWorkflowStatusRequest>(serializeEntityData)!;
        ClientDto dto = new ClientDto()
        {
            Id = workflowData.recordId,
            Tags = requestEntity.tags,
            Names = new List<MultilanguageText>(){new MultilanguageText(){
            Label=requestEntity.name,
            Language="en-EN"
         }},
            Status = workflowData.newStatus == "client-approve" ? "New"
        : workflowData.newStatus == "client-reject" || workflowData.newStatus == "client-deactive" ? "Deactive"
        : workflowData.newStatus == "client-active-fd" || workflowData.newStatus == "client-update-approve" ? "Active"
        : workflowData.newStatus,
            Secret = requestEntity.secret,
            ReturnUrl = requestEntity.returnUrl,
            LoginUrl = requestEntity.loginUrl,
            LogoutUrl = requestEntity.logoutUrl,
            Pkce = requestEntity.pkce,
            Tokens = requestEntity.tokens!.Select(tok => new ClientToken
            {
                Type = tok!.type!,
                DefaultDuration = tok.tokenDuration,
                PublicClaims = tok.publicClaims


            }).ToList(),
            Flows = requestEntity.flows!.Select(tok => new ClientFlow
            {
                Type = tok!.type,
                Workflow = tok.workflow,
                TokenDuration = tok.tokenDuration


            }).ToList(),
            AllowedGrantTypes = requestEntity.allowedGrantTypes!.Select(s => new ClientGrantType()
            {
                GrantType = s,
                ClientId = workflowData.recordId

            }).ToList(),
            Idempotency = new Idempotency()
            {
                Mode = requestEntity.idempotencyMode
            }
        };


        return await UpsertMethod(mapper, validator, context, bbtIdentity, dto, httpContext, token);
    }

    protected override async ValueTask<IResult> UpsertMethod(
    [FromServices] IMapper mapper,
    [FromServices] IValidator<Client> validator,
    [FromServices] UserDBContext context,
    [FromServices] IBBTIdentity bbtIdentity,
    [FromBody] ClientDto data,
    HttpContext httpContext,
    CancellationToken token
    )
    {
        var dbModelData = mapper.Map<Client>(data);

        FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(dbModelData);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        DbSet<Client> dbSet = context.Set<Client>();

        bool IsChange = false;
        Client? dataFromDB = await dbSet.FirstOrDefaultAsync(x => x.Id == dbModelData.Id, token);

        if (dataFromDB != null)
        {
            if (PropertyCheckList != null)
            {
                object? dbValue;
                object? dtoValue;

                foreach (string property in PropertyCheckList)
                {
                    dbValue = typeof(Client).GetProperties().First(p => p.Name.Equals(property)).GetValue(dataFromDB);
                    dtoValue = typeof(ClientDto).GetProperties().First(p => p.Name.Equals(property)).GetValue(data);

                    if (dbValue != null && !dbValue.Equals(dtoValue))
                    {
                        typeof(Client).GetProperties().First(p => p.Name.Equals(property)).SetValue(dataFromDB, dtoValue);
                        IsChange = true;
                    }
                }
            }

            if (IsChange)
            {
                dataFromDB.ModifiedAt = DateTime.UtcNow;
                dataFromDB.ModifiedBy = bbtIdentity.UserId.Value;
                dataFromDB.ModifiedByBehalfOf = bbtIdentity.BehalfOfId.Value;

                await context.SaveChangesAsync(token);
                return Results.Ok(mapper.Map<Client>(dataFromDB));
            }
            else
            {
                return Results.NoContent();
            }
        }
        else
        {
            dbModelData.CreatedAt = DateTime.UtcNow;
            dbModelData.CreatedBy = bbtIdentity.UserId.Value;
            dbModelData.CreatedByBehalfOf = bbtIdentity.BehalfOfId.Value;

            dbModelData.ModifiedAt = dbModelData.CreatedAt;
            dbModelData.ModifiedBy = dbModelData.CreatedBy;
            dbModelData.ModifiedByBehalfOf = dbModelData.CreatedByBehalfOf;


            string secret = Guid.NewGuid().ToString();
            dbModelData.Secret = ComputeSha256Hash(secret);

            await dbSet.AddAsync(dbModelData);
            await context.SaveChangesAsync(token);

            dbModelData.Secret = secret;

            return Results.Created($"/{dbModelData.Id}", mapper.Map<ClientGetDto>(dbModelData));
        }
    }

    string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

[AddSwaggerParameter("Language", ParameterLocation.Header, false)]
    async ValueTask<IResult> getAllClientFullTextSearch(
      [FromServices] UserDBContext context,
      [AsParameters] ClientSearch dataSearch,
      [FromServices] IMapper mapper,
      HttpContext httpContext
    )
    {
        var query = context!.Clients!
         .Include(t => t.HeaderConfig)
         .Include(t => t.Jws)
         .Include(t => t.Idempotency)
         .Include(t => t.Names)
         .Include(t => t.Tokens)
         .Include(t => t.AllowedGrantTypes)
         .Include(t => t.Flows)
         .Include(t => t.Names.Where(t => t.Language == httpContext.GetHeaderLanguage()))
            .Skip(dataSearch.Page * dataSearch.PageSize)
            .Take(dataSearch.PageSize);

        if (!string.IsNullOrEmpty(dataSearch.Keyword))
        {
            query = query.AsNoTracking().Where(x => EF.Functions.ToTsVector("english", string.Join(" ", x.ReturnUrl, x.LoginUrl, x.LogoutUrl))
           .Matches(EF.Functions.PlainToTsQuery("english", dataSearch.Keyword)));
        }

        var clients = query.ToList();

        if (clients.Count() > 0)
        {
            var response = clients.Select(x => mapper.Map<ClientGetDto>(x)).ToList();
            return Results.Ok(response);
        }

        return Results.NoContent();
    }

}