using amorphie.core.Module.minimal_api;
using amorphie.core.Repository;
using FluentValidation;
using amorphie.core.Base;
using amorphie.fact.data;

namespace amorphie.client;

    public abstract class BaseClientTokenModule<TDTOModel, TDBModel, TValidator>
        : BaseBBTRouteRepository<TDTOModel, TDBModel, TValidator, UserDBContext, IBBTRepository<TDBModel, UserDBContext>>
        where TDTOModel : class, new()
        where TDBModel : EntityBase
        where TValidator : AbstractValidator<TDBModel>
    {
        protected BaseClientTokenModule(WebApplication app) : base(app)
        {
        }

        public override string[]? PropertyCheckList => throw new NotImplementedException();

        public override string? UrlFragment => throw new NotImplementedException();

    }
