using BackendAkademija.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BackendAkademija.Application.Common.Behaviors;

public class ChachingBehaviour<TRequest, TResponse>(IMemoryCache cache, ILogger<ChachingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheable cacheableRequest)
            return await next(cancellationToken);
        
        var cacheKey = cacheableRequest.CacheKey;

        if (cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return cachedResponse;
        }
        
        logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
        var response = await next(cancellationToken);
        
        cache.Set(cacheKey, response, cacheableRequest.CacheDuration);
        return response;
    }
}