using BackendAkademija.Application.Dto;
using BackendAkademija.Application.Helper;
using BackendAkademija.Application.Interfaces;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.FilterProducts;

public class FilterProductsQueryHandler(IProductSource productSource) : IRequestHandler<FilterProductsQuery, IReadOnlyList<ProductListItemDto>>
{
    public async Task<IReadOnlyList<ProductListItemDto>> Handle(FilterProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productSource.GetAllAsync(cancellationToken);

        var query = products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(p =>
                p.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        return query
            .Select(p => new ProductListItemDto(
                p.Id, p.Title, p.Price, StringHelper.Truncate(p.Description, 100), p.Thumbnail))
            .ToList();
    }
}