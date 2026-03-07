using MediatR;
using CloudNative.AccountingService.Application.DTOs;

namespace CloudNative.AccountingService.Application.Commands.CreateAccountingItem;

public record CreateAccountingItemCommand(
    string  Name,
    string  Description,
    decimal Amount,
    string  Currency = "USD") : IRequest<AccountingItemDto>;
