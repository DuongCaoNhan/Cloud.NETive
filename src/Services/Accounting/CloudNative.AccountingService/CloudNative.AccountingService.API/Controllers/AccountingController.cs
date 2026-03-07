using MediatR;
using Microsoft.AspNetCore.Mvc;
using CloudNative.AccountingService.Application.Commands.CreateAccountingItem;
using CloudNative.AccountingService.Application.Commands.DeleteAccountingItem;
using CloudNative.AccountingService.Application.Commands.UpdateAccountingItem;
using CloudNative.AccountingService.Application.DTOs;
using CloudNative.AccountingService.Application.Queries.GetAccountingItem;
using CloudNative.AccountingService.Application.Queries.GetAllAccountingItems;
using CloudNative.Core.Models;

namespace CloudNative.AccountingService.API.Controllers;

/// <summary>Accounting items CRUD — Pure DDD sample: Controller talks ONLY to MediatR.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class AccountingController(IMediator mediator) : ControllerBase
{
    /// <summary>Get all accounting items.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AccountingItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllAccountingItemsQuery(), ct);
        return Ok(ApiResponse<IReadOnlyList<AccountingItemDto>>.Ok(result));
    }

    /// <summary>Get accounting item by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AccountingItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAccountingItemQuery(id), ct);
        return result is null
            ? NotFound(ApiResponse<AccountingItemDto>.Fail($"Item {id} not found."))
            : Ok(ApiResponse<AccountingItemDto>.Ok(result));
    }

    /// <summary>Create a new accounting item.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AccountingItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAccountingItemCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<AccountingItemDto>.Ok(result));
    }

    /// <summary>Update an existing accounting item.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AccountingItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountingItemCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command with { Id = id }, ct);
        return Ok(ApiResponse<AccountingItemDto>.Ok(result));
    }

    /// <summary>Delete an accounting item.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAccountingItemCommand(id), ct);
        return NoContent();
    }
}
