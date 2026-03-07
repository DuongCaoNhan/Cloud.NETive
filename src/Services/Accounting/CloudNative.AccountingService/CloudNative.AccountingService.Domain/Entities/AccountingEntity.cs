using CloudNative.AccountingService.Domain.Events;
using CloudNative.AccountingService.Domain.Exceptions;
using CloudNative.AccountingService.Domain.ValueObjects;
using CloudNative.Core.Entities;

namespace CloudNative.AccountingService.Domain.Entities;

/// <summary>
/// Aggregate Root — represents a budget/ledger line item.
/// Pure DDD: private setters, factory method, business behaviour, domain events.
/// </summary>
public class AccountingItem : AuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public string            Name        { get; private set; } = string.Empty;
    public string            Description { get; private set; } = string.Empty;
    public Money             Amount      { get; private set; } = Money.Zero();
    public AccountItemStatus Status      { get; private set; } = AccountItemStatus.Active;

    // EF Core materialisation constructor
    private AccountingItem() { }

    // ── Factory method ────────────────────────────────────────────────────
    public static AccountingItem Create(string name, string description, Money amount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new AccountingDomainException("Name cannot be empty.");

        var item = new AccountingItem
        {
            Name        = name.Trim(),
            Description = description.Trim(),
            Amount      = amount,
            Status      = AccountItemStatus.Active,
        };

        item._domainEvents.Add(new AccountingItemCreatedDomainEvent(item.Name, item.Amount));
        return item;
    }

    // ── Business methods ──────────────────────────────────────────────────
    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new AccountingDomainException("Name cannot be empty.");

        Name        = name.Trim();
        Description = description.Trim();
        UpdatedAt   = DateTime.UtcNow;
    }

    public void UpdateAmount(Money newAmount)
    {
        Amount    = newAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == AccountItemStatus.Active)
            throw new AccountingDomainException("Item is already active.");

        Status    = AccountItemStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (Status == AccountItemStatus.Inactive)
            throw new AccountingDomainException("Item is already inactive.");

        Status    = AccountItemStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new AccountingItemDeactivatedDomainEvent(Id, Name));
    }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
}

public enum AccountItemStatus { Active = 1, Inactive = 0 }
