using System.Reflection;
using BackendAkademija.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace BackendAkademija.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDi(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(ChachingBehaviour<,>));
        });
        
        services.AddValidatorsFromAssembly(assembly);
        services.AddMemoryCache();
        
        return services;
    }
}