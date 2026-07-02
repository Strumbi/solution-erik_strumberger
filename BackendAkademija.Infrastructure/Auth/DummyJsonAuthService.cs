using System.Net.Http.Json;
using BackendAkademija.Application.Dto.AuthDto;
using BackendAkademija.Application.Interfaces;
using BackendAkademija.Application.Models;

namespace BackendAkademija.Infrastructure;


internal record DummyJsonLoginRequest(string Username, string Password);
internal record DummyJsonLoginResponse(int Id, string Username);

public class DummyJsonAuthService(
    HttpClient httpClient, 
    IJwtTokenGenerator tokenGenerator,
    IRefreshTokenStore refreshTokenStore) : IAuthService
{
    public async Task<LoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new DummyJsonLoginRequest(username, password),
            cancellationToken
        );
        
        if(!response.IsSuccessStatusCode) return new LoginResult(false, null, null, "Login failed");

        var dummyUser = await response.Content.ReadFromJsonAsync<DummyJsonLoginResponse>(cancellationToken);

        if (dummyUser is null) return new LoginResult(false, null, null, "Bad response from data source");
        
        var accessToken = tokenGenerator.GenerateAccessToken(dummyUser.Id, dummyUser.Username);
        var refreshToken = tokenGenerator.GenerateRefreshToken(dummyUser.Id, dummyUser.Username);
        
        refreshTokenStore.Save(refreshToken);
        
        return new LoginResult(true, accessToken, refreshToken.Token, null);
    }

    public Task<RefreshResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var storedtoken = refreshTokenStore.Get(refreshToken);

        if (storedtoken is null || storedtoken.IsRevoked || storedtoken.ExpiresAt < DateTime.UtcNow)
            return Task.FromResult(new RefreshResult(false, null, null, "Refreshtoken is not valid"));

        refreshTokenStore.Revoke(refreshToken);

        var newAccessToken = tokenGenerator.GenerateAccessToken(storedtoken.UserId, storedtoken.UserName);
        var newRefreshToken = tokenGenerator.GenerateRefreshToken(
            storedtoken.UserId,
            storedtoken.UserName,
            storedtoken.OriginalLoginAt
        );

        refreshTokenStore.Save(newRefreshToken);
        return Task.FromResult(new RefreshResult(true, newAccessToken, newRefreshToken.Token, null));
    }
}