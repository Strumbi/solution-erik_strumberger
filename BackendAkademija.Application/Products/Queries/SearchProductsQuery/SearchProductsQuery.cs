using BackendAkademija.Application.Dto;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.SearchProductsQuery;

public record SearchProductsQuery(string SearchTerm) : IRequest<IReadOnlyList<ProductListItemDto>>;