using MediatR;
using CloudNative.AccountingService.Application.DTOs;
using CloudNative.AccountingService.Domain.Repositories;

namespace CloudNative.AccountingService.Application.Queries.GetAllAccountingItems;

public sealed class GetAllAccountingItemsQueryHandler(IAccountingRepository repository)
    : IRequestHandler<GetAllAccountingItemsQuery, IReadOnlyList<AccountingItemDto>>
{
    public async Task<IReadOnlyList<AccountingItemDto>> Handle(GetAllAccountingItemsQuery request, CancellationToken ct)
    {
        var items = await repository.ListAllAsync(ct);
        return items.Select(x => x.ToDto()).ToList();
    }
}
