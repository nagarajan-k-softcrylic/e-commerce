using AutoMapper;
using Ecommerce.InventoryService.Application.Contracts;
using Ecommerce.InventoryService.Domain.Exceptions;
using Ecommerce.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.InventoryService.Application.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string ProductCode,
    string ProductName,
    string Description,
    decimal Price,
    int AvailableQuantity
) : IRequest<ProductDto>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository repository,
        IMapper mapper,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new InventoryDomainException($"Product with ID '{request.Id}' was not found.");

        product.Update(
            request.ProductCode,
            request.ProductName,
            request.Description,
            request.Price,
            request.AvailableQuantity
        );

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} updated.", product.Id);

        return _mapper.Map<ProductDto>(product);
    }
}

