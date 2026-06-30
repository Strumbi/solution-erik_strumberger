using System.Net.Http.Json;
using BackendAkademija.Application.Interfaces;
using BackendAkademija.Domain.Entities;
using BackendAkademija.Infrastructure.ExternalServices.DummyJson.Models;

namespace BackendAkademija.Infrastructure.ExternalServices.DummyJson;

public class DummyJsonProductsSource(HttpClient httpClient) : IProductSource
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetFromJsonAsync<DummyJsonProductsResponse>("products?limit=0", cancellationToken);
        
        return response?.Products.Select(p => p.ToDomain()).ToList()
            ?? new List<Product>();
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"products/{id}", cancellationToken);
        
        if(!response.IsSuccessStatusCode) return null;
        
        var dto = await response.Content.ReadFromJsonAsync<DummyJsonProductDto>(cancellationToken);

        return dto?.ToDomain();
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await httpClient.GetFromJsonAsync<List<DummyJsonCategoryDto>>(
        "products/categories", cancellationToken);
        
        return categories?.Select(c => c.ToDomain()).ToList()
            ?? new List<Category>();
    }
}