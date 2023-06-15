using amorphie.core.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using amorphie.fact.data;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace amorphie.client;

public class ClientModule
    : BaseClientModule<ClientDto, Client, ClientValidator>
{
    private readonly IConfiguration _configuration;
    public ClientModule(WebApplication app,IConfiguration configuration) : base(app)
    {
        _configuration = configuration;
    }

    public override string[]? PropertyCheckList => new string[] { "Type", "Validations", "Status" };

    public override string? UrlFragment => "client";


    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {
        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapPost("validate", validateClient);
        routeGroupBuilder.MapGet("search", getAllClientFullTextSearch);
    }

    protected override async ValueTask<IResult> Get([FromServices] IBBTRepository<Client, UserDBContext> repository, [FromRoute(Name = "id")] Guid id)
    {
        return Results.Ok(_configuration["mockResponse"]);

        var client = repository.DbContext.Clients!
         .Include(t => t.HeaderConfig)
         .Include(t => t.Jws)
         .Include(t => t.Idempotency)
         .Include(t => t.Names)
         .Include(t => t.Tokens)
         .Include(t => t.AllowedGrantTypes)
         .FirstOrDefault(t => t.Id == id);

        var model = await repository.GetById(id);

        if (model is Client)
        {
            return TypedResults.Ok(model);
        }

        return TypedResults.NotFound();
    }


    protected override async ValueTask<IResult> GetAll([FromServices] IBBTRepository<Client, UserDBContext> repository,
            [FromQuery][Range(0, 100)] int page,
            [FromQuery][Range(5, 100)] int pageSize)
    {
        var resultList = await repository.DbContext!.Clients!
        .Include(t => t.HeaderConfig)
       .Include(t => t.Jws)
       .Include(t => t.Idempotency)
       .Include(t => t.Names)
       .Include(t => t.Tokens)
       .Include(t => t.AllowedGrantTypes)
       .Skip(page * pageSize)
       .Take(pageSize)
       .ToListAsync();

        if (resultList != null && resultList.Count() > 0)
        {
            return Results.Ok(resultList);
        }

        return Results.NoContent();
    }

    async ValueTask<IResult> validateClient([FromBody] ValidateClientRequest data,
         [FromServices] UserDBContext context)
    {
        return Results.Ok(_configuration["mockResponse"]);
        var hashedSecret = ComputeSha256Hash(data.Secret!);

        var client = context!.Clients!
            .Include(t => t.HeaderConfig)
            .Include(t => t.Jws)
            .Include(t => t.Idempotency)
            .Include(t => t.Names)
            .Include(t => t.Tokens)
            .FirstOrDefault(t => t.Id == data.ClientId
                && t.Secret == hashedSecret);

        if (client == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(client);
    }

    protected override async ValueTask<IResult> Upsert([FromServices] IMapper mapper,
    [FromServices] ClientValidator validator, [FromServices] IBBTRepository<Client, UserDBContext> repository, [FromBody] ClientDto data)
    {
        var dbModelData = mapper.Map<Client>(data);

        FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(dbModelData);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        bool IsChange = false;
        Client? dataFromDB = await repository.GetById(dbModelData.Id);

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
                await repository.SaveChangesAsync();

            return Results.NoContent();
        }
        else
        {
            string secret = Guid.NewGuid().ToString();
            dbModelData.Secret = ComputeSha256Hash(secret);

            await repository.Insert(dbModelData);

            dbModelData.Secret = secret;

            return Results.Created($"/{dbModelData.Id}", dbModelData);
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

    async ValueTask<IResult> getAllClientFullTextSearch(
      [FromServices] UserDBContext context,
      [AsParameters] ClientSearch dataSearch,
      [FromServices] IMapper mapper
    )
    {
        var query = context!.Clients!
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
            var response = clients.Select(x => mapper.Map<ClientDto>(x)).ToList();
            return Results.Ok(response);
        }

        return Results.NoContent();
    }

}