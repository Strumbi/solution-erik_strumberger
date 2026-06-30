using BackendAkademija.Application.Dto;
using BackendAkademija.Application.Helper;
using BackendAkademija.Application.Interfaces;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.SearchProductsQuery;

public class SearchProductsQueryHandler(IProductSource productSource) : IRequestHandler<SearchProductsQuery, IReadOnlyList<ProductListItemDto>>
{
    public async Task<IReadOnlyList<ProductListItemDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productSource.GetAllAsync(cancellationToken);
        
        return products
            .Where(p => p.Title.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
            .Select(p => new ProductListItemDto(
                p.Id, p.Title, p.Price, StringHelper.Truncate(p.Description, 100), p.Thumbnail))
            .ToList();
    }
}