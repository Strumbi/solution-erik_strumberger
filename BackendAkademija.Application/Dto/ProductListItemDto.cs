namespace BackendAkademija.Application.Dto;

public record ProductListItemDto(int Id,
    string Title,
    decimal Price,
    string ShortDescription,
    string Thumbnail);