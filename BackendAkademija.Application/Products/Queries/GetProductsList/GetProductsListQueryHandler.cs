using BackendAkademija.Application.Dto;
using BackendAkademija.Application.Helper;
using BackendAkademija.Application.Interfaces;
using MediatR;

namespace BackendAkademija.Application.Products.Queries;

public class GetProductsListQueryHandler(IProductSource productSource) : IRequestHandler<GetProductsListQuery, IReadOnlyList<ProductListItemDto>>
{
    public async Task<IReadOnlyList<ProductListItemDto>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
    {
        var products = await productSource.GetAllAsync(cancellationToken);
        
        return products.Select(p => new ProductListItemDto(
            p.Id,
            p.Title,
            p.Price,
            StringHelper.Truncate(p.Description, 100),
            p.Thumbnail
        )).ToList();
    }
}