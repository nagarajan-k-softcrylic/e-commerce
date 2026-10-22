using Ecommerce.InventoryService.Application.Contracts;
using Ecommerce.InventoryService.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.InventoryService.Application.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IProductRepository repository, ILogger<DeleteProductCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new InventoryDomainException($"Product with ID '{request.Id}' was not found.");

        await _repository.DeleteAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} deleted.", product.Id);
        return true;
    }
}

