using amorphie.core.Module.minimal_api;
using amorphie.core.Repository;
using FluentValidation;
using amorphie.core.Base;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using amorphie.fact.data;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace amorphie.client;

public class ClientModule
    : BaseClientModule<ClientDto, Client, ClientValidator>
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
    }

   async ValueTask<IResult> validateClient([FromBody] ValidateClientRequest data,
        [FromServices] UserDBContext context)
    {
        var hashedSecret = ComputeSha256Hash(data.Secret!);

        var client = context!.Clients!
            .Include(t => t.HeaderConfig)
            .Include(t => t.Jws)
            .Include(t => t.Idempotency)
            .Include(t => t.Names)
            .Include(t => t.Tokens)
            .FirstOrDefault(t => t.Id == data.ClientId
                && t.Secret == hashedSecret
                && (string.IsNullOrEmpty(t.ReturnUrl) || data.ReturnUrl == data.ReturnUrl));

        if (client == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(client);

    }


    protected override ValueTask<IResult> Upsert([FromServices] IMapper mapper,
    [FromServices] ClientValidator validator, [FromServices] IBBTRepository<Client, UserDBContext> repository, [FromBody] ClientDto data)
    {
        string secret = Guid.NewGuid().ToString();
        data.Secret = ComputeSha256Hash(secret);
        return base.Upsert(mapper, validator, repository, data);
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

}