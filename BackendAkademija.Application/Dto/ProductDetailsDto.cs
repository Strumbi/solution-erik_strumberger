namespace BackendAkademija.Application.Dto;

public record ProductDetailsDto(
    int Id,
    string Title,
    string Description,
    decimal Price,
    string Category,
    double Rating,
    int Stock,
    List<string> Tags,
    string Thumbnail,
    List<string> Images,
    string Brand);