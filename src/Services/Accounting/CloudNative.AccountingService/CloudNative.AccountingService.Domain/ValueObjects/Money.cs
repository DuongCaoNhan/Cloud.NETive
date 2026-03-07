namespace CloudNative.AccountingService.Domain.ValueObjects;

/// <summary>
/// Value Object — immutable, equality by value, no identity.
/// Represents a monetary amount with currency.
/// </summary>
public sealed class Money
{
    public decimal Amount   { get; }
    public string  Currency { get; }

    // EF Core uses this constructor via constructor-binding (param names match properties)
    private Money(decimal amount, string currency)
    {
        Amount   = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));

        return new Money(amount, currency.ToUpperInvariant());
    }

    public static Money Zero(string currency = "USD") => new(0, currency.ToUpperInvariant());

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add {Currency} and {other.Currency}.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract {Currency} and {other.Currency}.");
        return new Money(Amount - other.Amount, Currency);
    }

    public override bool Equals(object? obj) =>
        obj is Money m && Amount == m.Amount && Currency == m.Currency;

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public override string ToString() => $"{Amount:N2} {Currency}";
}
