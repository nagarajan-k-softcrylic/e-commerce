using Ecommerce.OrderService.Domain.Common;

namespace Ecommerce.OrderService.Domain.Entities;

public class OrderItem : BaseEntity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    public Order? Order { get; private set; }

    private OrderItem() { }

    public OrderItem(Guid id, Guid orderId, Guid productId, string productName, int quantity, decimal price)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Price = price;
    }
}

