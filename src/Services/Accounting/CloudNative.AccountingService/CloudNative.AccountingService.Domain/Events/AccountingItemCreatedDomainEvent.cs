using CloudNative.AccountingService.Domain.ValueObjects;

namespace CloudNative.AccountingService.Domain.Events;

public record AccountingItemCreatedDomainEvent(string Name, Money Amount) : IDomainEvent;
