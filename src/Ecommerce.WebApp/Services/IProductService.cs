using Ecommerce.Shared.DTOs;

namespace Ecommerce.WebApp.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetProductsAsync(string? search = null);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto?> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(Guid id);
}

