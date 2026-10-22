using Blazored.LocalStorage;
using Ecommerce.Shared.DTOs;
using Ecommerce.WebApp.Models;

namespace Ecommerce.WebApp.Services;

public class CartService
{
    private readonly ILocalStorageService _localStorage;
    private const string CartStorageKey = "shoppingCart";
    private readonly List<CartItem> _items = new();

    public event Action? OnChange;

    public CartService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var saved = await _localStorage.GetItemAsync<List<CartItem>>(CartStorageKey);
            if (saved != null)
            {
                _items.Clear();
                _items.AddRange(saved);
                OnChange?.Invoke();
            }
        }
        catch
        {
            // fallback
        }
    }

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public int TotalItemCount => _items.Sum(i => i.Quantity);
    public decimal TotalAmount => _items.Sum(i => i.SubTotal);

    public async Task AddItemAsync(ProductDto product, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = quantity
            });
        }

        await PersistAsync();
        OnChange?.Invoke();
    }

    public async Task RemoveItemAsync(Guid productId)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            _items.Remove(existing);
            await PersistAsync();
            OnChange?.Invoke();
        }
    }

    public async Task UpdateQuantityAsync(Guid productId, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            if (quantity <= 0)
            {
                _items.Remove(existing);
            }
            else
            {
                existing.Quantity = quantity;
            }
            await PersistAsync();
            OnChange?.Invoke();
        }
    }

    public async Task ClearCartAsync()
    {
        _items.Clear();
        await _localStorage.RemoveItemAsync(CartStorageKey);
        OnChange?.Invoke();
    }

    private async Task PersistAsync()
    {
        await _localStorage.SetItemAsync(CartStorageKey, _items);
    }
}

