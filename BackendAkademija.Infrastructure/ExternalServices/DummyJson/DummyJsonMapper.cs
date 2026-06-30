using BackendAkademija.Domain.Entities;
using BackendAkademija.Infrastructure.ExternalServices.DummyJson.Models;

namespace BackendAkademija.Infrastructure.ExternalServices.DummyJson;

public static class DummyJsonMapper
{
    public static Product ToDomain(this DummyJsonProductDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Description = dto.Description,
        Price = dto.Price,
        Category = dto.Category,
        Rating = dto.Rating,
        Stock = dto.Stock,
        Tags = dto.Tags,
        Thumbnail = dto.Thumbnail,
        Images = dto.Images,
        Brand = dto.Brand
    };

    public static Category ToDomain(this DummyJsonCategoryDto dto) => new()
    {
        Slug = dto.Slug,
        Name = dto.Name,
        Url = dto.Url
    };

}