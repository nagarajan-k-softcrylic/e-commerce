namespace Ecommerce.Shared.DTOs;

public record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}

public record OrderDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = "Pending";
    public DateTime CreatedDate { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
}

public record CreateOrderItemRequest(Guid ProductId, string ProductName, int Quantity, decimal Price);

public record CreateOrderRequest
{
    public string CustomerId { get; init; } = string.Empty;
    public List<CreateOrderItemRequest> Items { get; init; } = new();
}

public record CheckoutOrderRequest
{
    public string CustomerId { get; init; } = string.Empty;
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}

public record UpdateOrderStatusRequest(string Status);

