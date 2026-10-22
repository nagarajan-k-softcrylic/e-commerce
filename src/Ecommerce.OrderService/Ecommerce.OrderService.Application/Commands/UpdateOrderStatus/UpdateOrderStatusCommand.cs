using AutoMapper;
using Ecommerce.OrderService.Application.Contracts;
using Ecommerce.OrderService.Domain.Entities;
using Ecommerce.OrderService.Domain.Exceptions;
using Ecommerce.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Application.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid OrderId, string Status) : IRequest<OrderDto>;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository repository,
        IMapper mapper,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new OrderDomainException($"Order with ID '{request.OrderId}' not found.");

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var parsedStatus))
            throw new OrderDomainException($"Invalid order status '{request.Status}'. Valid statuses are: Pending, Confirmed, Failed, Cancelled.");

        order.UpdateStatus(parsedStatus);
        await _repository.UpdateAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} status updated to {Status}.", order.Id, order.Status);

        return _mapper.Map<OrderDto>(order);
    }
}

