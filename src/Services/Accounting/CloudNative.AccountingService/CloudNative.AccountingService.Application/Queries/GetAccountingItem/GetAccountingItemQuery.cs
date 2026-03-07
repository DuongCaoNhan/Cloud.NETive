using MediatR;
using CloudNative.AccountingService.Application.DTOs;

namespace CloudNative.AccountingService.Application.Queries.GetAccountingItem;

public record GetAccountingItemQuery(int Id) : IRequest<AccountingItemDto?>;
