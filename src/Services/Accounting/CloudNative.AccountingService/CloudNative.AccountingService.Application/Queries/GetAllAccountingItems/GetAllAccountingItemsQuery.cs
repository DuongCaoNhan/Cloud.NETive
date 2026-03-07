using MediatR;
using CloudNative.AccountingService.Application.DTOs;

namespace CloudNative.AccountingService.Application.Queries.GetAllAccountingItems;

public record GetAllAccountingItemsQuery : IRequest<IReadOnlyList<AccountingItemDto>>;
