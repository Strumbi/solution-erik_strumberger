using BackendAkademija.Application.Dto;
using BackendAkademija.Application.Interfaces;
using MediatR;

namespace BackendAkademija.Application.Products.Queries.GetCatrgories;

public class GetCategoriesQueryHandler(IProductSource source) : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await source.GetAllCategoriesAsync(cancellationToken);
        
        return categories
            .Select(c => new CategoryDto(c.Slug, c.Name, c.Url))
            .ToList();
    }
}