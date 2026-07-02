namespace BackendAkademija.Application.Models;

public record LoginResult(
    bool Success,
    string? AccessToken,
    string? RefreshToken,
    string? Error);