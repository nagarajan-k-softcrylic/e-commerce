using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Ecommerce.Shared.Bus;
using Ecommerce.Shared.Bus.AzureServiceBus;
using Ecommerce.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.InventoryService.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that consumes OrderCreatedEvent from Azure Service Bus,
/// validates stock, and publishes InventoryReservedEvent or InventoryFailedEvent.
/// </summary>
public class InventoryServiceEventConsumer : BackgroundService
{
    private readonly AzureServiceBusOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InventoryServiceEventConsumer> _logger;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public InventoryServiceEventConsumer(
        IOptions<AzureServiceBusOptions> options,
        IServiceProvider serviceProvider,
        ILogger<InventoryServiceEventConsumer> logger)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            _logger.LogInformation("Azure Service Bus connection string is empty. InventoryServiceEventConsumer will yield to In-Memory event bus.");
            return;
        }

        try
        {
            _client = new ServiceBusClient(_options.ConnectionString);
            _processor = _client.CreateProcessor(_options.OrderCreatedTopic, _options.OrderCreatedSubscription, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 5
            });

            _processor.ProcessMessageAsync += HandleMessageAsync;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(stoppingToken);
            _logger.LogInformation("InventoryService Azure Service Bus processor started for topic {Topic}, sub {Sub}",
                _options.OrderCreatedTopic, _options.OrderCreatedSubscription);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Azure Service Bus processor in InventoryService. Running in fallback mode.");
        }
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = Encoding.UTF8.GetString(args.Message.Body);
            var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (@event != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<OrderCreatedEvent>>();
                await handler.HandleAsync(@event, args.CancellationToken);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing OrderCreated message {MessageId}", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Azure Service Bus consumer error in {ErrorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor != null)
            await _processor.StopProcessingAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}

