using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Bus.InMemory;

/// <summary>
/// In-Memory asynchronous event bus channel for local development, fallback, and automated testing.
/// Seamlessly forwards events to IIntegrationEventHandler implementations within DI scopes.
/// </summary>
public class InMemoryEventBus : BackgroundService, IEventBus
{
    private readonly Channel<object> _channel = Channel.CreateUnbounded<object>(new UnboundedChannelOptions
    {
        SingleReader = false,
        SingleWriter = false
    });

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(IServiceProvider serviceProvider, ILogger<InMemoryEventBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("[InMemoryBus] Event published: {EventType}", typeof(T).Name);
        await _channel.Writer.WriteAsync(@event, cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[InMemoryBus] Event dispatcher started.");

        while (!stoppingToken.IsCancellationRequested && await _channel.Reader.WaitToReadAsync(stoppingToken))
        {
            while (_channel.Reader.TryRead(out var @event))
            {
                var eventType = @event.GetType();
                var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                using var scope = _serviceProvider.CreateScope();
                var handlers = scope.ServiceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    try
                    {
                        var method = handlerType.GetMethod(nameof(IIntegrationEventHandler<object>.HandleAsync));
                        if (method != null)
                        {
                            var task = (Task?)method.Invoke(handler, new[] { @event, stoppingToken });
                            if (task != null)
                            {
                                await task;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[InMemoryBus] Error handling event {EventType} by {Handler}",
                            eventType.Name, handler?.GetType().Name);
                    }
                }
            }
        }
    }
}

