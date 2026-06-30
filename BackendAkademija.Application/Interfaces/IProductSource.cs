using BackendAkademija.Domain.Entities;

namespace BackendAkademija.Application.Interfaces;

public interface IProductSource
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken);
}