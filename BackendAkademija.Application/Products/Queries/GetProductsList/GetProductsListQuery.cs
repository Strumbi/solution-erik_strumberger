using BackendAkademija.Application.Dto;
using MediatR;

namespace BackendAkademija.Application.Products.Queries;

public record GetProductsListQuery : IRequest<IReadOnlyList<ProductListItemDto>>;