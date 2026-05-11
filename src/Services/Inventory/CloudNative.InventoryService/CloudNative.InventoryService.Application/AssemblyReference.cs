using System.Reflection;

namespace CloudNative.InventoryService.Application;

/// <summary>
/// Assembly marker for MediatR and FluentValidation registration.
/// </summary>
public sealed class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
