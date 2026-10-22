using AutoMapper;
using Ecommerce.InventoryService.Application.Contracts;
using Ecommerce.Shared.DTOs;
using MediatR;

namespace Ecommerce.InventoryService.Application.Queries.GetProducts;

public record GetProductsQuery(string? SearchTerm = null) : IRequest<List<ProductDto>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLowerInvariant();
            products = products
                .Where(p => p.ProductName.ToLowerInvariant().Contains(term) ||
                            p.ProductCode.ToLowerInvariant().Contains(term) ||
                            p.Description.ToLowerInvariant().Contains(term))
                .ToList();
        }

        return _mapper.Map<List<ProductDto>>(products);
    }
}

