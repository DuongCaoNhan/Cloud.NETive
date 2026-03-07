using MediatR;
using CloudNative.AccountingService.Application.DTOs;
using CloudNative.AccountingService.Domain.Repositories;

namespace CloudNative.AccountingService.Application.Queries.GetAccountingItem;

public sealed class GetAccountingItemQueryHandler(IAccountingRepository repository)
    : IRequestHandler<GetAccountingItemQuery, AccountingItemDto?>
{
    public async Task<AccountingItemDto?> Handle(GetAccountingItemQuery request, CancellationToken ct)
    {
        var item = await repository.GetByIdAsync(request.Id, ct);
        return item?.ToDto();
    }
}
