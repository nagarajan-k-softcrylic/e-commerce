using AutoMapper;
using Ecommerce.OrderService.Application.Contracts;
using Ecommerce.Shared.DTOs;
using MediatR;

namespace Ecommerce.OrderService.Application.Queries.GetOrders;

public record GetOrdersQuery(string? CustomerId = null) : IRequest<List<OrderDto>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = string.IsNullOrWhiteSpace(request.CustomerId)
            ? await _repository.GetAllAsync(cancellationToken)
            : await _repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        return _mapper.Map<List<OrderDto>>(orders);
    }
}

