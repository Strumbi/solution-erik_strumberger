using BackendAkademija.Application.Interfaces;
using BackendAkademija.Infrastructure.Auth;
using BackendAkademija.Infrastructure.ExternalServices.DummyJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendAkademija.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastractureDi(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["DummyJson:BaseUrl"];

        services.AddHttpClient<IProductSource, DummyJsonProductsSource>(client =>
        {
            if (baseUrl != null) client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<IAuthService, DummyJsonAuthService>(client =>
        {
            if(baseUrl != null) client.BaseAddress = new Uri(baseUrl);
        });
        

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        
        return services;

    }
}