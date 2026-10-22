namespace Ecommerce.Shared.DTOs;

public record ProductDto
{
    public Guid Id { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int AvailableQuantity { get; init; }
    public DateTime CreatedDate { get; init; }
}

public record CreateProductDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int AvailableQuantity { get; init; }
}

public record UpdateProductDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int AvailableQuantity { get; init; }
}

public record ReserveInventoryRequest(Guid OrderId, Guid ProductId, int Quantity);
public record ReleaseInventoryRequest(Guid OrderId, Guid ProductId, int Quantity);
public record InventoryOperationResult(bool Success, string Message, int RemainingQuantity);

