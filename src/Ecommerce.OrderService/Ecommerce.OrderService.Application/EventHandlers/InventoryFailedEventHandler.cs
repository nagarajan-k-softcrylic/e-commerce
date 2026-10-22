using Ecommerce.OrderService.Application.Contracts;
using Ecommerce.Shared.Bus;
using Ecommerce.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Application.EventHandlers;

public class InventoryFailedEventHandler : IIntegrationEventHandler<InventoryFailedEvent>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<InventoryFailedEventHandler> _logger;

    public InventoryFailedEventHandler(
        IOrderRepository repository,
        ILogger<InventoryFailedEventHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(InventoryFailedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Received InventoryFailedEvent for Order {OrderId}, Product {ProductId}. Reason: {Reason}",
            @event.OrderId, @event.ProductId, @event.Reason);

        var order = await _repository.GetByIdAsync(@event.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found while handling InventoryFailedEvent", @event.OrderId);
            return;
        }

        order.Fail(@event.Reason);
        await _repository.UpdateAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} status updated to Failed. Reason: {Reason}", @event.OrderId, @event.Reason);
    }
}

