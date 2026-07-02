namespace BackendAkademija.Application.Interfaces;

public interface ICacheable
{
    string CacheKey { get; }
    TimeSpan CacheDuration { get; }
}