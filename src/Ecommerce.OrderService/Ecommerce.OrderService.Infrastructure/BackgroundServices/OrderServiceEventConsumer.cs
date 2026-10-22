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

namespace Ecommerce.OrderService.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that consumes Azure Service Bus events:
/// 1. InventoryReservedEvent -> marks Order as Confirmed
/// 2. InventoryFailedEvent -> marks Order as Failed
/// </summary>
public class OrderServiceEventConsumer : BackgroundService
{
    private readonly AzureServiceBusOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderServiceEventConsumer> _logger;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _reservedProcessor;
    private ServiceBusProcessor? _failedProcessor;

    public OrderServiceEventConsumer(
        IOptions<AzureServiceBusOptions> options,
        IServiceProvider serviceProvider,
        ILogger<OrderServiceEventConsumer> logger)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            _logger.LogInformation("Azure Service Bus connection string is empty. OrderServiceEventConsumer will yield to In-Memory event bus.");
            return;
        }

        try
        {
            _client = new ServiceBusClient(_options.ConnectionString);

            _reservedProcessor = _client.CreateProcessor(_options.InventoryReservedTopic, _options.InventoryReservedSubscription, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 5
            });
            _reservedProcessor.ProcessMessageAsync += HandleReservedMessageAsync;
            _reservedProcessor.ProcessErrorAsync += ErrorHandler;

            _failedProcessor = _client.CreateProcessor(_options.InventoryFailedTopic, _options.InventoryFailedSubscription, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 5
            });
            _failedProcessor.ProcessMessageAsync += HandleFailedMessageAsync;
            _failedProcessor.ProcessErrorAsync += ErrorHandler;

            await _reservedProcessor.StartProcessingAsync(stoppingToken);
            await _failedProcessor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("OrderService Azure Service Bus consumers started successfully for topics: {Topic1}, {Topic2}",
                _options.InventoryReservedTopic, _options.InventoryFailedTopic);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Azure Service Bus processors in OrderService. Running in fallback mode.");
        }
    }

    private async Task HandleReservedMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = Encoding.UTF8.GetString(args.Message.Body);
            var @event = JsonSerializer.Deserialize<InventoryReservedEvent>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (@event != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<InventoryReservedEvent>>();
                await handler.HandleAsync(@event, args.CancellationToken);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing InventoryReserved message {MessageId}", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private async Task HandleFailedMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = Encoding.UTF8.GetString(args.Message.Body);
            var @event = JsonSerializer.Deserialize<InventoryFailedEvent>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (@event != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<InventoryFailedEvent>>();
                await handler.HandleAsync(@event, args.CancellationToken);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing InventoryFailed message {MessageId}", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Azure Service Bus consumer encountered an error in {ErrorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_reservedProcessor != null)
            await _reservedProcessor.StopProcessingAsync(cancellationToken);
        if (_failedProcessor != null)
            await _failedProcessor.StopProcessingAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}

