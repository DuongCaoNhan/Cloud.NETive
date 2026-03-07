using MediatR;
using CloudNative.AccountingService.Application.DTOs;
using CloudNative.AccountingService.Domain.Entities;
using CloudNative.AccountingService.Domain.Repositories;
using CloudNative.AccountingService.Domain.ValueObjects;

namespace CloudNative.AccountingService.Application.Commands.CreateAccountingItem;

public sealed class CreateAccountingItemCommandHandler(IAccountingRepository repository)
    : IRequestHandler<CreateAccountingItemCommand, AccountingItemDto>
{
    public async Task<AccountingItemDto> Handle(CreateAccountingItemCommand request, CancellationToken ct)
    {
        var money = Money.Create(request.Amount, request.Currency);
        var item  = AccountingItem.Create(request.Name, request.Description, money);

        await repository.AddAsync(item, ct);

        item.ClearDomainEvents(); // events dispatched; clear before returning
        return item.ToDto();
    }
}
