using CreacionFeatures.Domain.Entities;
using CreacionFeatures.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

namespace CreacionFeatures.Tests;

public class InMemoryProductRepositoryTests
{
    private readonly InMemoryProductRepository _repository;

    public InMemoryProductRepositoryTests()
    {
        _repository = new InMemoryProductRepository();
    }

    [Fact]
    public async Task GetAllAsync_EmptyStore_ReturnsEmpty()
    {
        var result = await _repository.GetAllAsync();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAsync_AddsProduct_ReturnsProduct()
    {
        var product = new Product("Test", "Desc", 10m);
        var added = await _repository.AddAsync(product);
        added.Should().Be(product);
    }

    [Fact]
    public async Task GetAllAsync_AfterAdd_ReturnsAllProducts()
    {
        await _repository.AddAsync(new Product("P1", "D1", 1m));
        await _repository.AddAsync(new Product("P2", "D2", 2m));

        var result = await _repository.GetAllAsync();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
    {
        var product = new Product("Find Me", "Desc", 5m);
        await _repository.AddAsync(product);

        var found = await _repository.GetByIdAsync(product.Id);
        found.Should().NotBeNull();
        found!.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesProduct_ReturnsUpdated()
    {
        var product = new Product("Old Name", "Old Desc", 1m);
        await _repository.AddAsync(product);
        product.Update("New Name", "New Desc", 99m);

        var updated = await _repository.UpdateAsync(product);
        updated.Name.Should().Be("New Name");

        var fromStore = await _repository.GetByIdAsync(product.Id);
        fromStore!.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesProduct()
    {
        var product = new Product("To Remove", "Desc", 1m);
        await _repository.AddAsync(product);

        await _repository.DeleteAsync(product.Id);

        var found = await _repository.GetByIdAsync(product.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_DoesNotThrow()
    {
        Func<Task> act = () => _repository.DeleteAsync(Guid.NewGuid());
        await act.Should().NotThrowAsync();
    }
}
