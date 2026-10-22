using AutoMapper;
using Ecommerce.OrderService.Application.Contracts;
using Ecommerce.OrderService.Domain.Entities;
using Ecommerce.Shared.Bus;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Application.Commands.CheckoutOrder;

public record CheckoutOrderCommand(
    string CustomerId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Price
) : IRequest<OrderDto>;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, OrderDto>
{
    private readonly IOrderRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler(
        IOrderRepository repository,
        IEventBus eventBus,
        IMapper mapper,
        ILogger<CheckoutOrderCommandHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
        var order = new Order(Guid.NewGuid(), orderNumber, request.CustomerId);
        order.AddItem(request.ProductId, request.ProductName, request.Quantity, request.Price);

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} checked out in Pending status. Total: {Total:C}", order.Id, order.TotalAmount);

        // Publish OrderCreatedEvent to Azure Service Bus
        var @event = new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            TotalAmount = order.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        await _eventBus.PublishAsync(@event, cancellationToken);
        _logger.LogInformation("Published OrderCreatedEvent to Azure Service Bus for Order {OrderId}", order.Id);

        return _mapper.Map<OrderDto>(order);
    }
}

