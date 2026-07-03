using System.Net.Http.Json;
using Docker.DotNet.Models;

namespace BackendAkademija.IntegrationTests.Infrastructure;

public class AuthHelper
{
    public static async Task<string> GetAccessTokenAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "emilys",
            password = "emilyspass"
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return result!.AccessToken;
    }
    
    private record AuthResponse(string AccessToken, string RefreshToken);
}