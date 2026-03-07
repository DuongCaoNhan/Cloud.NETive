using MediatR;
using CloudNative.AccountingService.Application.DTOs;

namespace CloudNative.AccountingService.Application.Commands.UpdateAccountingItem;

public record UpdateAccountingItemCommand(
    int     Id,
    string  Name,
    string  Description,
    decimal Amount,
    string  Currency = "USD") : IRequest<AccountingItemDto>;
