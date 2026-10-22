namespace Ecommerce.Shared.Bus.AzureServiceBus;

public class AzureServiceBusOptions
{
    public const string SectionName = "AzureServiceBus";

    public string ConnectionString { get; set; } = string.Empty;
    public string OrderCreatedTopic { get; set; } = "order-created-topic";
    public string OrderCreatedSubscription { get; set; } = "inventory-service-sub";
    public string InventoryReservedTopic { get; set; } = "inventory-reserved-topic";
    public string InventoryReservedSubscription { get; set; } = "order-service-sub";
    public string InventoryFailedTopic { get; set; } = "inventory-failed-topic";
    public string InventoryFailedSubscription { get; set; } = "order-service-sub";
    public bool UseInMemoryFallbackIfEmpty { get; set; } = true;
}

