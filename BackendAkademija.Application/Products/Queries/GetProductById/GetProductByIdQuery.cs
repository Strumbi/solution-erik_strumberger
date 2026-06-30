using BackendAkademija.Application.Dto;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int id) : IRequest<ProductDetailsDto>;