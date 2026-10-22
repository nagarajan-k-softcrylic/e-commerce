namespace Ecommerce.Shared.Events;

/// <summary>
/// Event published by InventoryService when stock validation fails (e.g. out of stock).
/// Triggers OrderService to transition order status to Failed.
/// </summary>
public record InventoryFailedEvent
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime FailedAt { get; init; } = DateTime.UtcNow;
}

