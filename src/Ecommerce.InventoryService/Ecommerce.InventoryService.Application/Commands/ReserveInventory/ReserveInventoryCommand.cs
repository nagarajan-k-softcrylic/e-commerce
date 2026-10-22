using Ecommerce.InventoryService.Application.Contracts;
using Ecommerce.InventoryService.Domain.Exceptions;
using Ecommerce.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.InventoryService.Application.Commands.ReserveInventory;

public record ReserveInventoryCommand(Guid OrderId, Guid ProductId, int Quantity) : IRequest<InventoryOperationResult>;

public class ReserveInventoryCommandHandler : IRequestHandler<ReserveInventoryCommand, InventoryOperationResult>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ReserveInventoryCommandHandler> _logger;

    public ReserveInventoryCommandHandler(IProductRepository repository, ILogger<ReserveInventoryCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<InventoryOperationResult> Handle(ReserveInventoryCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new InventoryDomainException($"Product with ID '{request.ProductId}' was not found.");

        product.ReserveStock(request.Quantity);

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully reserved {Quantity} units of {ProductName} for Order {OrderId}. Remaining: {Remaining}",
            request.Quantity, product.ProductName, request.OrderId, product.AvailableQuantity);

        return new InventoryOperationResult(true, "Inventory reserved successfully.", product.AvailableQuantity);
    }
}

