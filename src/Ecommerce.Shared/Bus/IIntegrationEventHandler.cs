namespace Ecommerce.Shared.Bus;

/// <summary>
/// Abstraction for handling incoming integration events.
/// </summary>
public interface IIntegrationEventHandler<in TEvent> where TEvent : class
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

