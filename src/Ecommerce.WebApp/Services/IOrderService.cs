using Ecommerce.Shared.DTOs;

namespace Ecommerce.WebApp.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(Guid id);
    Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderDto?> CheckoutAsync(CheckoutOrderRequest request);
    Task<OrderDto?> UpdateOrderStatusAsync(Guid id, string status);
}

