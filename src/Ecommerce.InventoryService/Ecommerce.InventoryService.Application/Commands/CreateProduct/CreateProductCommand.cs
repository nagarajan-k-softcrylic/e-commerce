using AutoMapper;
using Ecommerce.InventoryService.Application.Contracts;
using Ecommerce.InventoryService.Domain.Entities;
using Ecommerce.InventoryService.Domain.Exceptions;
using Ecommerce.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.InventoryService.Application.Commands.CreateProduct;

public record CreateProductCommand(
    string ProductCode,
    string ProductName,
    string Description,
    decimal Price,
    int AvailableQuantity
) : IRequest<ProductDto>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByCodeAsync(request.ProductCode, cancellationToken);
        if (existing != null)
            throw new InventoryDomainException($"Product with code '{request.ProductCode}' already exists.");

        var product = new Product(
            Guid.NewGuid(),
            request.ProductCode,
            request.ProductName,
            request.Description,
            request.Price,
            request.AvailableQuantity
        );

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} ({ProductCode} - {ProductName}) created.",
            product.Id, product.ProductCode, product.ProductName);

        return _mapper.Map<ProductDto>(product);
    }
}

