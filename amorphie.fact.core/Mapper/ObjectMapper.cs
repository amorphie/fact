using System;
using AutoMapper;

public class ObjectMapper
{
    private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<UserMapper>();
        cfg.AddProfile<SecurityImageMapper>();
        cfg.AddProfile<UserDeviceMapper>();
        cfg.AddProfile<SecurityQuestionMapper>();
        cfg.AddProfile<UserSecurityImageMapper>();
        cfg.AddProfile<UserSecurityQuestionMapper>();
        cfg.AddProfile<UserTagMapper>();
        cfg.AddProfile<ClientMapper>();
        cfg.AddProfile<ClientFlowMapper>();
        cfg.AddProfile<ClientGrantTypeMapper>();
        cfg.AddProfile<ClientAudienceMapper>();
        cfg.AddProfile<JwsMapper>();
        cfg.AddProfile<IdempotencyMapper>();
        cfg.AddProfile<ClientTokenMapper>();
        cfg.AddProfile<UserClaimMapper>();
    });

    return config.CreateMapper();
});

    public static IMapper Mapper => lazy.Value;
}