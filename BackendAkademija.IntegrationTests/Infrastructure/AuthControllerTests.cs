using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BackendAkademija.IntegrationTests.Infrastructure;

public class AuthControllerTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Login_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "emilys",
            password = "emilyspass"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<TokenResponse>();
        content.Should().NotBeNull();
        content!.AccessToken.Should().NotBeNullOrEmpty();
        content.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenCredentialsAreInvalid()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "wrong",
            password = "wrong"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        // Arrange - login prvo
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "emilys",
            password = "emilyspass"
        });

        var tokens = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();

        // Act
        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = tokens!.RefreshToken
        });

        // Assert
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var newTokens = await refreshResponse.Content.ReadFromJsonAsync<TokenResponse>();
        newTokens!.AccessToken.Should().NotBeNullOrEmpty();
        newTokens.RefreshToken.Should().NotBe(tokens.RefreshToken); // rotation
    }

    [Fact]
    public async Task Refresh_ShouldReturn401_WhenRefreshTokenUsedTwice()
    {
        // Arrange - login i prvi refresh
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "emilys",
            password = "emilyspass"
        });

        var tokens = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();

        await _client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = tokens!.RefreshToken
        });

        // Act - drugi refresh s istim tokenom
        var secondRefresh = await _client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = tokens.RefreshToken
        });

        // Assert
        secondRefresh.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record TokenResponse(string AccessToken, string RefreshToken);
}