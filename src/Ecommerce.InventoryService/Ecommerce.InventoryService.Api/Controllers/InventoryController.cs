using Ecommerce.InventoryService.Application.Commands.ReleaseInventory;
using Ecommerce.InventoryService.Application.Commands.ReserveInventory;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.InventoryService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IMediator mediator, ILogger<InventoryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Manually reserve inventory for an order item.
    /// </summary>
    [HttpPost("reserve")]
    [Authorize]
    [ProducesResponseType(typeof(InventoryOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reserve([FromBody] ReserveInventoryRequest request, CancellationToken cancellationToken)
    {
        var command = new ReserveInventoryCommand(request.OrderId, request.ProductId, request.Quantity);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Release previously reserved inventory back to available stock.
    /// </summary>
    [HttpPost("release")]
    [Authorize]
    [ProducesResponseType(typeof(InventoryOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Release([FromBody] ReleaseInventoryRequest request, CancellationToken cancellationToken)
    {
        var command = new ReleaseInventoryCommand(request.OrderId, request.ProductId, request.Quantity);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

