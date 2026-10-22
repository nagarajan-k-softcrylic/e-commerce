using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Ecommerce.Shared.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Shared.Bus.AzureServiceBus;

/// <summary>
/// Production Azure Service Bus publisher implementation.
/// </summary>
public class AzureServiceBusEventBus : IEventBus, IAsyncDisposable
{
    private readonly AzureServiceBusOptions _options;
    private readonly ILogger<AzureServiceBusEventBus> _logger;
    private readonly ServiceBusClient _client;
    private readonly Dictionary<string, ServiceBusSender> _senders = new();
    private readonly object _lock = new();

    public AzureServiceBusEventBus(
        IOptions<AzureServiceBusOptions> options,
        ILogger<AzureServiceBusEventBus> logger)
    {
        _options = options.Value;
        _logger = logger;
        _client = new ServiceBusClient(_options.ConnectionString);
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        var topicName = ResolveTopicName<T>();
        var sender = GetOrCreateSender(topicName);

        var payload = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(payload))
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
            Subject = typeof(T).Name
        };

        message.ApplicationProperties["MessageType"] = typeof(T).AssemblyQualifiedName;

        _logger.LogInformation("Publishing event {EventType} to Azure Service Bus topic {TopicName}. MessageId: {MessageId}",
            typeof(T).Name, topicName, message.MessageId);

        await sender.SendMessageAsync(message, cancellationToken);

        _logger.LogInformation("Successfully published event {EventType} to topic {TopicName}",
            typeof(T).Name, topicName);
    }

    private ServiceBusSender GetOrCreateSender(string topicOrQueue)
    {
        lock (_lock)
        {
            if (!_senders.TryGetValue(topicOrQueue, out var sender))
            {
                sender = _client.CreateSender(topicOrQueue);
                _senders[topicOrQueue] = sender;
            }
            return sender;
        }
    }

    private string ResolveTopicName<T>()
    {
        return typeof(T).Name switch
        {
            nameof(OrderCreatedEvent) => _options.OrderCreatedTopic,
            nameof(InventoryReservedEvent) => _options.InventoryReservedTopic,
            nameof(InventoryFailedEvent) => _options.InventoryFailedTopic,
            _ => typeof(T).Name.ToLowerInvariant()
        };
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var sender in _senders.Values)
        {
            await sender.DisposeAsync();
        }
        await _client.DisposeAsync();
    }
}

