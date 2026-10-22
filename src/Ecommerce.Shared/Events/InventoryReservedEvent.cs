namespace Ecommerce.Shared.Events;

/// <summary>
/// Event published by InventoryService when stock validation succeeds and items are reserved.
/// Triggers OrderService to transition order status to Confirmed.
/// </summary>
public record InventoryReservedEvent
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public DateTime ReservedAt { get; init; } = DateTime.UtcNow;
}

