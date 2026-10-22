using Ecommerce.OrderService.Domain.Common;
using Ecommerce.OrderService.Domain.Exceptions;

namespace Ecommerce.OrderService.Domain.Entities;

public class Order : BaseEntity<Guid>, IAggregateRoot
{
    private readonly List<OrderItem> _items = new();

    public string OrderNumber { get; private set; } = string.Empty;
    public string CustomerId { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? FailureReason { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public Order(Guid id, string orderNumber, string customerId)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new OrderDomainException("Order number is required.");
        if (string.IsNullOrWhiteSpace(customerId))
            throw new OrderDomainException("Customer id is required.");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        OrderNumber = orderNumber;
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedDate = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new OrderDomainException("Quantity must be greater than zero.");
        if (price < 0)
            throw new OrderDomainException("Price cannot be negative.");

        var item = new OrderItem(Guid.NewGuid(), Id, productId, productName, quantity, price);
        _items.Add(item);
        RecalculateTotal();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException($"Cannot confirm order in status '{Status}'. Only Pending orders can be confirmed.");

        Status = OrderStatus.Confirmed;
    }

    public void Fail(string reason)
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException($"Cannot mark order as failed from status '{Status}'.");

        Status = OrderStatus.Failed;
        FailureReason = reason;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Confirmed)
            throw new OrderDomainException("Confirmed orders cannot be cancelled directly without refund processing.");

        Status = OrderStatus.Cancelled;
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.Quantity * i.Price);
    }
}

