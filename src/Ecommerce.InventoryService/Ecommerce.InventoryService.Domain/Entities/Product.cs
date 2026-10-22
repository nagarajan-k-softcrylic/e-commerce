using Ecommerce.InventoryService.Domain.Common;
using Ecommerce.InventoryService.Domain.Exceptions;

namespace Ecommerce.InventoryService.Domain.Entities;

public class Product : BaseEntity<Guid>, IAggregateRoot
{
    public string ProductCode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int AvailableQuantity { get; private set; }
    public DateTime CreatedDate { get; private set; }

    private Product() { }

    public Product(Guid id, string productCode, string productName, string description, decimal price, int availableQuantity)
    {
        Validate(productCode, productName, price, availableQuantity);

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        ProductCode = productCode;
        ProductName = productName;
        Description = description ?? string.Empty;
        Price = price;
        AvailableQuantity = availableQuantity;
        CreatedDate = DateTime.UtcNow;
    }

    public void Update(string productCode, string productName, string description, decimal price, int availableQuantity)
    {
        Validate(productCode, productName, price, availableQuantity);

        ProductCode = productCode;
        ProductName = productName;
        Description = description ?? string.Empty;
        Price = price;
        AvailableQuantity = availableQuantity;
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new InventoryDomainException("Reserved quantity must be greater than zero.");

        if (AvailableQuantity < quantity)
            throw new InventoryDomainException($"Insufficient stock for '{ProductName}'. Available: {AvailableQuantity}, Requested: {quantity}.");

        AvailableQuantity -= quantity;
    }

    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new InventoryDomainException("Released quantity must be greater than zero.");

        AvailableQuantity += quantity;
    }

    private static void Validate(string productCode, string productName, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new InventoryDomainException("ProductCode is required.");
        if (string.IsNullOrWhiteSpace(productName))
            throw new InventoryDomainException("ProductName is required.");
        if (price < 0)
            throw new InventoryDomainException("Price cannot be negative.");
        if (quantity < 0)
            throw new InventoryDomainException("AvailableQuantity cannot be negative.");
    }
}

