using BackendAkademija.Application.Dto;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.GetCatrgories;

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;