using System.Net.Http.Json;
using BackendAkademija.Application.Interfaces;
using BackendAkademija.Domain.Entities;
using BackendAkademija.Infrastructure.ExternalServices.DummyJson.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BackendAkademija.Infrastructure.ExternalServices.DummyJson;

public class DummyJsonProductsSource(HttpClient httpClient, IMemoryCache cache, ILogger<DummyJsonProductsSource> logger) : IProductSource
{
    
    private const string AllProductsCacheKey = "products:all";
    private static readonly TimeSpan AllProductsCacheDuration = TimeSpan.FromMinutes(5);
    
    private const string AllCategoriesCacheKey = "categories:all";
    private static readonly TimeSpan AllCategoriesCacheDuration = TimeSpan.FromMinutes(30);
    
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(AllProductsCacheKey, out IReadOnlyList<Product>? cachedProducts) &&
            cachedProducts is not null)
        {
            logger.LogInformation("Product source cache hit for {CacheKey}", AllProductsCacheKey);
            return cachedProducts;
        }
        
        var response = await httpClient.GetFromJsonAsync<DummyJsonProductsResponse>("products?limit=0", cancellationToken);
        var products = response?.Products.Select(p => p.ToDomain()).ToList() ?? new List<Product>();
        
        cache.Set(AllProductsCacheKey, (IReadOnlyList<Product>)products, AllProductsCacheDuration);
        return products;
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
        if (cache.TryGetValue(AllCategoriesCacheKey, out IReadOnlyList<Category>? cachedCategories) && cachedCategories is not null)
        {
            logger.LogInformation("Product source cache hit for {CacheKey}", AllCategoriesCacheKey);
            return cachedCategories;
        }

        logger.LogInformation("Product source cache miss for {CacheKey}, fetching from DummyJSON", AllCategoriesCacheKey);

        var categories = await httpClient.GetFromJsonAsync<List<DummyJsonCategoryDto>>("products/categories", cancellationToken);

        var result = categories?.Select(c => c.ToDomain()).ToList() ?? new List<Category>();

        cache.Set(AllCategoriesCacheKey, (IReadOnlyList<Category>)result, AllCategoriesCacheDuration);

        return result;
    }
}