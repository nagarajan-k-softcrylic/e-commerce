using AutoMapper;
using Ecommerce.OrderService.Application.Contracts;
using Ecommerce.OrderService.Domain.Entities;
using Ecommerce.Shared.Bus;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Application.Commands.CreateOrder;

public record CreateOrderCommand(string CustomerId, List<CreateOrderItemRequest> Items) : IRequest<OrderDto>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        IEventBus eventBus,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
        var order = new Order(Guid.NewGuid(), orderNumber, request.CustomerId);

        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.Quantity, item.Price);
        }

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} ({OrderNumber}) created in Pending status.", order.Id, order.OrderNumber);

        // Publish OrderCreatedEvent for each item to validate stock with Inventory Service
        foreach (var item in order.Items)
        {
            var @event = new OrderCreatedEvent
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                TotalAmount = order.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _eventBus.PublishAsync(@event, cancellationToken);
            _logger.LogInformation("Published OrderCreatedEvent for Order {OrderId}, Product {ProductId}", order.Id, item.ProductId);
        }

        return _mapper.Map<OrderDto>(order);
    }
}

