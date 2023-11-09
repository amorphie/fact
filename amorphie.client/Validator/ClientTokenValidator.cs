using FluentValidation;

public sealed class ClientTokenValidator : AbstractValidator<ClientToken>
{
    public ClientTokenValidator()
    {
        // RuleFor(x => x.Name).NotNull();
        // RuleFor(x => x.Name).MinimumLength(10);
    }
}