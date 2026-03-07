namespace CloudNative.AccountingService.Domain.Events;

public record AccountingItemDeactivatedDomainEvent(int Id, string Name) : IDomainEvent;
