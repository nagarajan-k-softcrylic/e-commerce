using Ecommerce.InventoryService.Application.Contracts;
using Ecommerce.Shared.Bus;
using Ecommerce.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Ecommerce.InventoryService.Application.EventHandlers;

public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    private readonly IProductRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        IProductRepository repository,
        IEventBus eventBus,
        ILogger<OrderCreatedEventHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderCreatedEvent for Order {OrderId}, Product {ProductId}, Quantity {Quantity}",
            @event.OrderId, @event.ProductId, @event.Quantity);

        var product = await _repository.GetByIdAsync(@event.ProductId, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found for Order {OrderId}. Publishing InventoryFailedEvent.",
                @event.ProductId, @event.OrderId);

            var failedEvent = new InventoryFailedEvent
            {
                OrderId = @event.OrderId,
                ProductId = @event.ProductId,
                Reason = $"Product with ID '{@event.ProductId}' does not exist in inventory.",
                FailedAt = DateTime.UtcNow
            };

            await _eventBus.PublishAsync(failedEvent, cancellationToken);
            return;
        }

        // Validate stock
        if (product.AvailableQuantity < @event.Quantity)
        {
            _logger.LogWarning("Insufficient stock for Product {ProductName}. Available: {Available}, Requested: {Requested}. Publishing InventoryFailedEvent.",
                product.ProductName, product.AvailableQuantity, @event.Quantity);

            var failedEvent = new InventoryFailedEvent
            {
                OrderId = @event.OrderId,
                ProductId = @event.ProductId,
                Reason = $"Insufficient stock for '{product.ProductName}'. Available: {product.AvailableQuantity}, Requested: {@event.Quantity}.",
                FailedAt = DateTime.UtcNow
            };

            await _eventBus.PublishAsync(failedEvent, cancellationToken);
            return;
        }

        // Reserve stock
        product.ReserveStock(@event.Quantity);
        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Stock reserved for Order {OrderId}, Product {ProductName}. Remaining: {Remaining}. Publishing InventoryReservedEvent.",
            @event.OrderId, product.ProductName, product.AvailableQuantity);

        var reservedEvent = new InventoryReservedEvent
        {
            OrderId = @event.OrderId,
            ProductId = @event.ProductId,
            Quantity = @event.Quantity,
            ReservedAt = DateTime.UtcNow
        };

        await _eventBus.PublishAsync(reservedEvent, cancellationToken);
    }
}

