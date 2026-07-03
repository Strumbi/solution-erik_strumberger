using BackendAkademija.Application.Dto;
using BackendAkademija.Application.Interfaces;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int id) : IRequest<ProductDetailsDto?>, ICacheable
{
    public string CacheKey => $"product_{id}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(10);
}