using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace BackendAkademija.IntegrationTests.Infrastructure;

public class ProductsControllerTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var token = await AuthHelper.GetAccessTokenAsync(_client);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetProducts_ShouldReturn401_WhenNotAuthenticated()
    {
        var response = await _client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnProductList_WhenAuthenticated()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductListItem>>();
        products.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/products/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDetails>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn404_WhenProductDoesNotExist()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/products/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FilterProducts_ShouldReturnFilteredProducts_ByCategory()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/products/filter?category=smartphones");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductListItem>>();
        products.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task FilterProducts_ShouldReturn400_WhenMinPriceGreaterThanMaxPrice()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/products/filter?minPrice=500&maxPrice=100");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchProducts_ShouldReturnResults_WhenSearchTermMatches()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/products/search?searchTerm=phone");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductListItem>>();
        products.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SearchProducts_ShouldReturn400_WhenSearchTermIsEmpty()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/products/search?searchTerm=");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private record ProductListItem(int Id, string Title, decimal Price, string ShortDescription, string Thumbnail);

    private record ProductDetails(int Id, string Title, string Description, decimal Price, string Category);
}