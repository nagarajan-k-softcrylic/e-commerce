using AutoMapper;
using Ecommerce.InventoryService.Domain.Entities;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.InventoryService.Application.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}

