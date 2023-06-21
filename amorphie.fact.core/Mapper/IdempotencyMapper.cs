using amorphie.core.Base;
using AutoMapper;
public class IdempotencyMapper : Profile
{
    public IdempotencyMapper()
    {
        CreateMap<Idempotency, IdempotencyDto>().ReverseMap();
        CreateMap<Idempotency, IdempotencyGetDto>().ReverseMap();
    }
}