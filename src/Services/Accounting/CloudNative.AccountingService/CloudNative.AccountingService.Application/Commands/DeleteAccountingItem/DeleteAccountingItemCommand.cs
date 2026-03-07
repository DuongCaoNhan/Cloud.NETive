using MediatR;

namespace CloudNative.AccountingService.Application.Commands.DeleteAccountingItem;

public record DeleteAccountingItemCommand(int Id) : IRequest;
