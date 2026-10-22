using Ecommerce.OrderService.Application.Contracts;
using Ecommerce.Shared.Bus;
using Ecommerce.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Application.EventHandlers;

public class InventoryReservedEventHandler : IIntegrationEventHandler<InventoryReservedEvent>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<InventoryReservedEventHandler> _logger;

    public InventoryReservedEventHandler(
        IOrderRepository repository,
        ILogger<InventoryReservedEventHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(InventoryReservedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received InventoryReservedEvent for Order {OrderId}, Product {ProductId}",
            @event.OrderId, @event.ProductId);

        var order = await _repository.GetByIdAsync(@event.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found while handling InventoryReservedEvent", @event.OrderId);
            return;
        }

        order.Confirm();
        await _repository.UpdateAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} successfully updated to Confirmed status via Azure Service Bus event.", @event.OrderId);
    }
}

