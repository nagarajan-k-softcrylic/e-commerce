using System.Security.Claims;
using Ecommerce.OrderService.Application.Commands.CheckoutOrder;
using Ecommerce.OrderService.Application.Commands.CreateOrder;
using Ecommerce.OrderService.Application.Commands.UpdateOrderStatus;
using Ecommerce.OrderService.Application.Queries.GetOrderById;
using Ecommerce.OrderService.Application.Queries.GetOrders;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders. Admins see all; Customers see their own orders.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        string? customerFilter = userRole == Roles.Admin ? null : userEmail;

        var result = await _mediator.Send(new GetOrdersQuery(customerFilter), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get order details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        if (result == null)
            return NotFound(new { message = $"Order with ID {id} was not found." });

        return Ok(result);
    }

    /// <summary>
    /// Create a new order with items.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var customerId = string.IsNullOrWhiteSpace(request.CustomerId)
            ? (User.FindFirstValue(ClaimTypes.Email) ?? "customer@ecommerce.com")
            : request.CustomerId;

        var command = new CreateOrderCommand(customerId, request.Items);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Checkout single product or cart order. Saves pending order & publishes OrderCreatedEvent.
    /// </summary>
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutOrderRequest request, CancellationToken cancellationToken)
    {
        var customerId = string.IsNullOrWhiteSpace(request.CustomerId)
            ? (User.FindFirstValue(ClaimTypes.Email) ?? "customer@ecommerce.com")
            : request.CustomerId;

        var command = new CheckoutOrderCommand(
            customerId,
            request.ProductId,
            request.ProductName,
            request.Quantity,
            request.Price
        );

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update order status manually (Admin only).
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateOrderStatusCommand(id, request.Status);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

