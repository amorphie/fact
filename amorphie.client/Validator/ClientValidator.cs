using FluentValidation;

public sealed class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator()
    {
        // RuleFor(x => x.Name).NotNull();
        // RuleFor(x => x.Name).MinimumLength(10);
    }
}