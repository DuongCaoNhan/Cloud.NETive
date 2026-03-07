using CloudNative.AccountingService.Domain.Entities;

namespace CloudNative.AccountingService.Application.DTOs;

/// <summary>Data Transfer Object — the only type the API layer ever sees.</summary>
public record AccountingItemDto(
    int      Id,
    string   Name,
    string   Description,
    decimal  Amount,
    string   Currency,
    string   Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

/// <summary>Mapping extension — keeps mapping logic in Application, away from Domain and API.</summary>
public static class AccountingItemMappings
{
    public static AccountingItemDto ToDto(this AccountingItem item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.Amount.Amount,
        item.Amount.Currency,
        item.Status.ToString(),
        item.CreatedAt,
        item.UpdatedAt);
}
