using FluentValidation;

namespace CloudNative.AccountingService.Application.Commands.CreateAccountingItem;

public sealed class CreateAccountingItemCommandValidator : AbstractValidator<CreateAccountingItemCommand>
{
    public CreateAccountingItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Amount cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3).WithMessage("Currency must be a 3-letter ISO code (e.g. USD).");
    }
}
