using FluentValidation;

namespace CloudNative.AccountingService.Application.Commands.UpdateAccountingItem;

public sealed class UpdateAccountingItemCommandValidator : AbstractValidator<UpdateAccountingItemCommand>
{
    public UpdateAccountingItemCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}
