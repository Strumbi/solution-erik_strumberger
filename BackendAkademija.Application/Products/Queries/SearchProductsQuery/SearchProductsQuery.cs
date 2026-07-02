using BackendAkademija.Application.Dto;
using BackendAkademija.Application.Interfaces;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.SearchProductsQuery;

public record SearchProductsQuery(string SearchTerm) : IRequest<IReadOnlyList<ProductListItemDto>>, ICacheable
{
    public string CacheKey => $"search_{SearchTerm.ToLower()}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}