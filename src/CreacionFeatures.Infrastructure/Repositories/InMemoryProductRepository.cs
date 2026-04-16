using System.Collections.Concurrent;
using CreacionFeatures.Domain.Entities;
using CreacionFeatures.Domain.Interfaces;

namespace CreacionFeatures.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _store = new();

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(_store.Values.ToList());
    }

    public Task<Product?> GetByIdAsync(Guid id)
    {
        _store.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<Product> AddAsync(Product product)
    {
        _store[product.Id] = product;
        return Task.FromResult(product);
    }

    public Task<Product> UpdateAsync(Product product)
    {
        _store[product.Id] = product;
        return Task.FromResult(product);
    }

    public Task DeleteAsync(Guid id)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
