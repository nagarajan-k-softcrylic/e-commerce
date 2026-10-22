using System.Net.Http.Json;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.WebApp.Services;

public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/orders") ?? new List<OrderDto>();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<OrderDto>($"api/orders/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/orders", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }
        return null;
    }

    public async Task<OrderDto?> CheckoutAsync(CheckoutOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/orders/checkout", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }
        return null;
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(Guid id, string status)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}/status", new UpdateOrderStatusRequest(status));
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        }
        return null;
    }
}

