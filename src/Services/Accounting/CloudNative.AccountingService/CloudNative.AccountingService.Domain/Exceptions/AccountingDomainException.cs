namespace CloudNative.AccountingService.Domain.Exceptions;

/// <summary>Thrown when a domain invariant is violated.</summary>
public sealed class AccountingDomainException(string message) : Exception(message);
