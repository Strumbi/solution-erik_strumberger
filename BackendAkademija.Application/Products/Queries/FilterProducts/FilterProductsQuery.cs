using BackendAkademija.Application.Dto;
using BackendAkademija.Application.Interfaces;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.FilterProducts;

public record FilterProductsQuery(
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice) : IRequest<IReadOnlyList<ProductListItemDto>>, ICacheable
{
    public string CacheKey => $"filter_{Category?.ToLower()}_{MinPrice}_{MaxPrice}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}