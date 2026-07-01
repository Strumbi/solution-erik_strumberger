using System.Net.Http.Json;
using BackendAkademija.Application.Interfaces;

namespace BackendAkademija.Infrastructure;


internal record DummyJsonLoginRequest(string Username, string Password);
internal record DummyJsonLoginResponse(int Id, string Username);

public class DummyJsonAuthService(HttpClient httpClient, IJwtTokenGenerator tokenGenerator) : IAuthService
{
    public async Task<LoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new DummyJsonLoginRequest(username, password),
            cancellationToken
        );
        
        if(!response.IsSuccessStatusCode) return new LoginResult(false, null, "Login failed");

        var dummyUser = await response.Content.ReadFromJsonAsync<DummyJsonLoginResponse>(cancellationToken);

        if (dummyUser is null) return new LoginResult(false, null, "Bad response from data source");
        
        var token = tokenGenerator.GenerateToken(dummyUser.Id, dummyUser.Username);
        
        return new LoginResult(true, token, null);
    }
}