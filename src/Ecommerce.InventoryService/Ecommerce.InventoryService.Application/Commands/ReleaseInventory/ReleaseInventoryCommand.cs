using Ecommerce.InventoryService.Application.Contracts;
using Ecommerce.InventoryService.Domain.Exceptions;
using Ecommerce.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.InventoryService.Application.Commands.ReleaseInventory;

public record ReleaseInventoryCommand(Guid OrderId, Guid ProductId, int Quantity) : IRequest<InventoryOperationResult>;

public class ReleaseInventoryCommandHandler : IRequestHandler<ReleaseInventoryCommand, InventoryOperationResult>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ReleaseInventoryCommandHandler> _logger;

    public ReleaseInventoryCommandHandler(IProductRepository repository, ILogger<ReleaseInventoryCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<InventoryOperationResult> Handle(ReleaseInventoryCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new InventoryDomainException($"Product with ID '{request.ProductId}' was not found.");

        product.ReleaseStock(request.Quantity);

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully released {Quantity} units of {ProductName} for Order {OrderId}. Remaining: {Remaining}",
            request.Quantity, product.ProductName, request.OrderId, product.AvailableQuantity);

        return new InventoryOperationResult(true, "Inventory released successfully.", product.AvailableQuantity);
    }
}

