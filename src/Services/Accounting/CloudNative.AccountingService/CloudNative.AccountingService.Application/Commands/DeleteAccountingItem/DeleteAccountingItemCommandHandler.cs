using MediatR;
using CloudNative.AccountingService.Domain.Exceptions;
using CloudNative.AccountingService.Domain.Repositories;

namespace CloudNative.AccountingService.Application.Commands.DeleteAccountingItem;

public sealed class DeleteAccountingItemCommandHandler(IAccountingRepository repository)
    : IRequestHandler<DeleteAccountingItemCommand>
{
    public async Task Handle(DeleteAccountingItemCommand request, CancellationToken ct)
    {
        var item = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new AccountingDomainException($"AccountingItem {request.Id} not found.");

        await repository.DeleteAsync(item, ct);
    }
}
