using AutoMapper;
using Ecommerce.OrderService.Domain.Entities;
using Ecommerce.Shared.DTOs;

namespace Ecommerce.OrderService.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));

        CreateMap<OrderItem, OrderItemDto>();
    }
}

