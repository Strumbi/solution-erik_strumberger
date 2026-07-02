namespace BackendAkademija.Application.Models;

public record RefreshResult(
    bool Success,
    string? AccessToken,
    string? RefreshToken,
    string? Error);