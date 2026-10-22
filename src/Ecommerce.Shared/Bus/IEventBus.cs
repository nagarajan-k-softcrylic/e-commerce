namespace Ecommerce.Shared.Bus;

/// <summary>
/// Abstraction for publishing integration events across microservices.
/// </summary>
public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
}

