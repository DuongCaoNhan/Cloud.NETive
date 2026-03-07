using MediatR;
using CloudNative.AccountingService.Application.DTOs;
using CloudNative.AccountingService.Domain.Exceptions;
using CloudNative.AccountingService.Domain.Repositories;
using CloudNative.AccountingService.Domain.ValueObjects;

namespace CloudNative.AccountingService.Application.Commands.UpdateAccountingItem;

public sealed class UpdateAccountingItemCommandHandler(IAccountingRepository repository)
    : IRequestHandler<UpdateAccountingItemCommand, AccountingItemDto>
{
    public async Task<AccountingItemDto> Handle(UpdateAccountingItemCommand request, CancellationToken ct)
    {
        var item = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new AccountingDomainException($"AccountingItem {request.Id} not found.");

        item.UpdateDetails(request.Name, request.Description);
        item.UpdateAmount(Money.Create(request.Amount, request.Currency));

        await repository.UpdateAsync(item, ct);
        return item.ToDto();
    }
}
