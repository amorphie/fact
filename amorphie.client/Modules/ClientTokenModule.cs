using System.ComponentModel.DataAnnotations;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
using amorphie.fact.data;
using Microsoft.AspNetCore.Mvc;

public static class ClientTokenModule
{
    public static void MapClientTokenEndpoints(this WebApplication app)
    {
        //getAllClientTokens
        app.MapGet("/clientToken", getAllClientTokens)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Returns all client tokens.";
                operation.Parameters[0].Description = "Paging parameter. **limit** is the page size of resultset.";
                operation.Parameters[1].Description = "Paging parameter. **Token** is returned from last query.";
                return operation;
            })
         .Produces<ClientTokenDto>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status204NoContent);

        //getClientToken
        app.MapGet("/clientToken/{clientTokenId}", getClientToken)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Returns requested client token.";
                operation.Parameters[0].Description = "Id of the requested client token.";
                return operation;
            })
            .Produces<ClientTokenDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        //saveClientToken
        app.MapPost("/clientToken", saveClientToken)
       .WithTopic("pubsub", "saveClientToken")
                .WithOpenApi(operation =>
                {
                    operation.Summary = "Saves or updates requested client token.";
                    return operation;
                })
                .Produces<ClientTokenDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status201Created);

        //deleteClientToken
        app.MapDelete("/clientToken/{clientTokenId}", deleteClientToken)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Deletes existing client token.";
                return operation;
            })
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent);        
    }

    static IResponse<List<ClientTokenDto>> getAllClientTokens(
        [FromServices] UserDBContext context,
        [FromQuery][Range(0, 100)] int page = 0,
        [FromQuery][Range(5, 100)] int pageSize = 100
        )
    {
        var clientTokens = context!.ClientTokens!
            .Skip(page * pageSize).Take(pageSize)
            .AsQueryable().ToList();

        if (clientTokens.Count == 0)
        {
            return new Response<List<ClientTokenDto>>
            {
                Data = null,
                Result = new amorphie.core.Base.Result(Status.Success, "Veri bulunamadı")
            };
        }

        return new Response<List<ClientTokenDto>>
        {
            Data = clientTokens.Select(x => ObjectMapper.Mapper.Map<ClientTokenDto>(x)).ToList(),
            Result = new amorphie.core.Base.Result(Status.Success, "Getirme başarılı")
        };
    }

    static IResponse<ClientTokenDto> getClientToken(
       [FromRoute(Name = "clientTokenId")] Guid clientTokenId,
       [FromServices] UserDBContext context
       )
    {
        var clientToken = context!.ClientTokens!
            .FirstOrDefault(t => t.Id == clientTokenId);

        if (clientToken == null)
        {
            return new Response<ClientTokenDto>
            {
                Data = null,
                Result = new amorphie.core.Base.Result(Status.Success, "Veri bulunamadı")
            };
        }

        return new Response<ClientTokenDto>
        {
            Data = ObjectMapper.Mapper.Map<ClientTokenDto>(clientToken),
            Result = new amorphie.core.Base.Result(Status.Success, "Getirme başarılı")
        };
    }

     static IResponse<ClientTokenDto> saveClientToken(
        [FromBody] SaveClientTokenRequest data,
        [FromServices] UserDBContext context
        )
    {
        ClientToken? existingRecord = null;

        if (data.Id == null)
        {
            data.Id = Guid.NewGuid();
        }
        else
        {
            existingRecord = context?.ClientTokens!
            .FirstOrDefault(t => t.Id == data.Id);
        }

        if (existingRecord == null)
        {
            var clientToken = ObjectMapper.Mapper.Map<ClientToken>(data);
            clientToken.CreatedAt = DateTime.UtcNow;
            context!.ClientTokens!.Add(clientToken);
            context.SaveChanges();
            
            return new Response<ClientTokenDto>
            {
                Data = ObjectMapper.Mapper.Map<ClientTokenDto>(clientToken),
                Result = new amorphie.core.Base.Result(Status.Success, "Kaydedildi")
            };
        }
        else
        {
            if (CheckForUpdate(data, existingRecord!, context!))
            {
                context!.SaveChanges();

                return new Response<ClientTokenDto>
                {
                    Data = ObjectMapper.Mapper.Map<ClientTokenDto>(existingRecord),
                    Result = new amorphie.core.Base.Result(Status.Success, "Güncelleme Başarili")
                };
            }

            return new Response<ClientTokenDto>
            {
                Data = ObjectMapper.Mapper.Map<ClientTokenDto>(existingRecord),
                Result = new Result(Status.Error, "Değişiklik yok")
            };
        }
    }

    static bool CheckForUpdate(SaveClientTokenRequest data, ClientToken existingRecord, UserDBContext context)
    {
        var hasChanges = false;

        if (data.Type != existingRecord.Type)
        {
            existingRecord.Type = data.Type;
            hasChanges = true;
        }        

        if (hasChanges)
        {
            existingRecord.ModifiedAt = DateTime.Now.ToUniversalTime();
            return true;
        }
        else
        {
            return false;
        }
    }

     static IResponse deleteClientToken(
    [FromRoute(Name = "clientTokenId")] Guid clientTokenId,
    [FromServices] UserDBContext context)
    {
        var existingRecord = context?.ClientTokens!
            .FirstOrDefault(t => t.Id == clientTokenId);

        if (existingRecord == null)
        {
            return new Response
            {
                Result = new amorphie.core.Base.Result(Status.Error, "Kayıt bulunumadı")
            };
        }
        else
        {
            context!.Remove(existingRecord);
            context.SaveChanges();

            return new Response
            {
                Result = new amorphie.core.Base.Result(Status.Error, "Silme başarılı")
            };
        }
    }
}