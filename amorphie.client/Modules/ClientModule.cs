using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.core.IBase;
using amorphie.fact.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ClientModule
{
    public static void MapClientEndpoints(this WebApplication app)
    {
        //getAllClients
        app.MapGet("/client", getAllClients)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Returns all clients.";
                operation.Parameters[0].Description = "Paging parameter. **limit** is the page size of resultset.";
                operation.Parameters[1].Description = "Paging parameter. **Token** is returned from last query.";
                operation.Parameters[2].Description = "RFC 5646 compliant language code.";
                return operation;
            })
         .Produces<ClientDto>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status204NoContent);

        //getClient
        app.MapGet("/client/{clientId}", getClient)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Returns requested client.";
                operation.Parameters[0].Description = "Id of the requested client.";
                return operation;
            })
            .Produces<ClientDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        //saveClient
        app.MapPost("/client", saveClient)
       .WithTopic("pubsub", "saveClient")
                .WithOpenApi(operation =>
                {
                    operation.Summary = "Saves or updates requested client.";
                    return operation;
                })
                .Produces<ClientSaveDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status201Created);

        //deleteClient
        app.MapDelete("/client/{clientId}", deleteClient)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Deletes existing client.";
                return operation;
            })
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent);

        //valideClient
        app.MapPost("/client/validate/", validateClient)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Validate requested client.";
                return operation;
            })
            .Produces<ClientSaveDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    static IResponse<List<ClientDto>> getAllClients(
        [FromServices] UserDBContext context,
        [FromQuery][Range(0, 100)] int page = 0,
        [FromQuery][Range(5, 100)] int pageSize = 100,
        [FromHeader(Name = "Language")] string? language = "en-EN"
        )
    {
        var clients = context!.Clients!
            .Include(t => t.HeaderConfig)
            .Include(t => t.Names.Where(t => t.Language == language))
            .Skip(page * pageSize).Take(pageSize)
            .AsQueryable().ToList();

        if (clients.Count == 0)
        {
            return new Response<List<ClientDto>>
            {
                Data = null,
                Result = new amorphie.core.Base.Result(Status.Success, "Veri bulunamadı")
            };
        }

        return new Response<List<ClientDto>>
        {
            Data = clients.Select(x => ObjectMapper.Mapper.Map<ClientDto>(x)).ToList(),
            Result = new amorphie.core.Base.Result(Status.Success, "Getirme başarılı")
        };
    }

    static IResponse<ClientDto> getClient(
       [FromRoute(Name = "clientId")] Guid clientId,
       [FromServices] UserDBContext context
       )
    {
        var client = context!.Clients!
            .Include(t => t.HeaderConfig)
            .Include(t => t.Names)
            .FirstOrDefault(t => t.Id == clientId);

        if (client == null)
        {
            return new Response<ClientDto>
            {
                Data = null,
                Result = new amorphie.core.Base.Result(Status.Success, "Veri bulunamadı")
            };
        }

        return new Response<ClientDto>
        {
            Data = ObjectMapper.Mapper.Map<ClientDto>(client),
            Result = new amorphie.core.Base.Result(Status.Success, "Getirme başarılı")
        };
    }

    static IResponse<ClientSaveDto> saveClient(
        [FromBody] SaveClientRequest data,
        [FromServices] UserDBContext context
        )
    {
        Client? existingRecord = null;

        if (data.Id == null)
        {
            data.Id = Guid.NewGuid();
        }
        else
        {
            existingRecord = context?.Clients!
            .Include(t => t.HeaderConfig)
            .Include(t => t.Names)
            .FirstOrDefault(t => t.Id == data.Id);
        }

        if (existingRecord == null)
        {
            var client = ObjectMapper.Mapper.Map<Client>(data);
            string secret = Guid.NewGuid().ToString();
            client.Secret = ComputeSha256Hash(secret);
            client.CreatedAt = DateTime.UtcNow;
            context!.Clients!.Add(client);
            context.SaveChanges();

            // To return unencrpted secret only once
            client.Secret = secret;
            return new Response<ClientSaveDto>
            {
                Data = ObjectMapper.Mapper.Map<ClientSaveDto>(client),
                Result = new amorphie.core.Base.Result(Status.Success, "Kaydedildi")
            };
        }
        else
        {
            if (CheckForUpdate(data, existingRecord!, context!))
            {
                context!.SaveChanges();

                return new Response<ClientSaveDto>
                {
                    Data = ObjectMapper.Mapper.Map<ClientSaveDto>(existingRecord),
                    Result = new amorphie.core.Base.Result(Status.Success, "Güncelleme Başarili")
                };
            }

            return new Response<ClientSaveDto>
            {
                Data = ObjectMapper.Mapper.Map<ClientSaveDto>(existingRecord),
                Result = new Result(Status.Error, "Değişiklik yok")
            };
        }
    }

    static bool CheckForUpdate(SaveClientRequest data, Client existingRecord, UserDBContext context)
    {
        var hasChanges = false;

        if (data.Type != existingRecord.Type)
        {
            existingRecord.Type = data.Type;
            hasChanges = true;
        }

        if (data.Validations != null && data.Validations != existingRecord.Validations)
        {
            existingRecord.Validations = data.Validations;
            hasChanges = true;
        }

        if (data.AvailableFlows != null && data.AvailableFlows != existingRecord.AvailableFlows)
        {
            existingRecord.AvailableFlows = data.AvailableFlows;
            hasChanges = true;
        }

        if (data.Status != null && data.Status != existingRecord.Status)
        {
            existingRecord.Status = data.Status;
            hasChanges = true;
        }

        if (data.Tags != null && data.Tags != existingRecord.Tags)
        {
            existingRecord.Tags = data.Tags;
            hasChanges = true;
        }

        foreach (MultilanguageText multilanguageText in data.Names)
        {
            var existingName = existingRecord.Names.FirstOrDefault(t => t.Language == multilanguageText.Language);

            if (existingName == null)
            {
                existingRecord.Names!.Add(ObjectMapper.Mapper.Map<Translation>(multilanguageText));
                var existingName2 = existingRecord.Names!.FirstOrDefault(t => t.Language == multilanguageText.Language);
                context.Add(existingName2);

                hasChanges = true;
            }
            else
            {
                if (existingName.Label != multilanguageText.Label)
                {
                    existingName.Label = multilanguageText.Label;
                    hasChanges = true;
                }
            }
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

    static IResponse deleteClient(
    [FromRoute(Name = "clientId")] Guid clientId,
    [FromServices] UserDBContext context)
    {
        var existingRecord = context?.Clients!
            .Include(t => t.HeaderConfig)
            .Include(t => t.Names)
            .FirstOrDefault(t => t.Id == clientId);

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

    static IResponse<ClientDto> validateClient(
        [FromBody] ValidateClientRequest data,
        [FromServices] UserDBContext context
       )
    {
        var hashedSecret = ComputeSha256Hash(data.Secret);

        var client = context!.Clients!        
            .Include(t => t.HeaderConfig)
            .Include(t => t.Names)
            .Include(t => t.Tokens)
            .FirstOrDefault(t => t.Id == data.ClientId 
                && t.Secret == hashedSecret 
                && (string.IsNullOrEmpty(t.ReturnUrl) || data.ReturnUrl == data.ReturnUrl));

        if (client == null)
        {
            return new Response<ClientDto>
            {
                Data = null,
                Result = new amorphie.core.Base.Result(Status.Success, "Veri bulunamadı")
            };
        }

        return new Response<ClientDto>
        {
            Data = ObjectMapper.Mapper.Map<ClientDto>(client),
            Result = new amorphie.core.Base.Result(Status.Success, "Getirme başarılı")
        };
    }
    static string ComputeSha256Hash(string rawData)
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