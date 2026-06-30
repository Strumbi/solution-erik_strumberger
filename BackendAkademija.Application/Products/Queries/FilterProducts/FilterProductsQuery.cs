using BackendAkademija.Application.Dto;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.FilterProducts;

public record FilterProductsQuery(
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice) : IRequest<IReadOnlyList<ProductListItemDto>>;