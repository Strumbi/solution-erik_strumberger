using BackendAkademija.Application.Dto;
using BackendAkademija.Application.Interfaces;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(IProductSource productSource) : IRequestHandler<GetProductByIdQuery, ProductDetailsDto?>
{
    public async Task<ProductDetailsDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productSource.GetByIdAsync(request.id, cancellationToken);
        if (product is null) return null;

        return new ProductDetailsDto(
            product.Id,
            product.Title,
            product.Description,
            product.Price,
            product.Category,
            product.Rating,
            product.Stock,
            product.Tags,
            product.Thumbnail,
            product.Images,
            product.Brand
        );
    }
}