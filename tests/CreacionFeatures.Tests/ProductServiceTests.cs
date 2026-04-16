using CreacionFeatures.Application.DTOs;
using CreacionFeatures.Application.Services;
using CreacionFeatures.Domain.Entities;
using CreacionFeatures.Domain.Exceptions;
using CreacionFeatures.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CreacionFeatures.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repoMock = new Mock<IProductRepository>();
        _service = new ProductService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new("Product A", "Desc A", 10.0m),
            new("Product B", "Desc B", 20.0m)
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
    {
        var product = new Product("Test", "Desc", 5.0m);
        _repoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

        var result = await _service.GetByIdAsync(product.Id);

        result.Id.Should().Be(product.Id);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ThrowsProductNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

        Func<Task> act = () => _service.GetByIdAsync(id);

        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedProduct()
    {
        var dto = new CreateProductDto { Name = "New", Description = "New Desc", Price = 15.0m };
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

        var result = await _service.CreateAsync(dto);

        result.Name.Should().Be("New");
        result.Price.Should().Be(15.0m);
    }

    [Fact]
    public async Task CreateAsync_EmptyName_ThrowsValidationException()
    {
        var dto = new CreateProductDto { Name = "", Price = 10m };

        Func<Task> act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*name cannot be empty*");
    }

    [Fact]
    public async Task CreateAsync_WhitespaceName_ThrowsValidationException()
    {
        var dto = new CreateProductDto { Name = "   ", Price = 10m };

        Func<Task> act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*name cannot be empty*");
    }

    [Fact]
    public async Task CreateAsync_NegativePrice_ThrowsValidationException()
    {
        var dto = new CreateProductDto { Name = "Valid", Price = -1m };

        Func<Task> act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*price cannot be negative*");
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ReturnsUpdatedProduct()
    {
        var existing = new Product("Old", "Old Desc", 5.0m);
        var dto = new UpdateProductDto { Name = "Updated", Description = "Updated Desc", Price = 25.0m };
        _repoMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);

        var result = await _service.UpdateAsync(existing.Id, dto);

        result.Name.Should().Be("Updated");
        result.Price.Should().Be(25.0m);
    }

    [Fact]
    public async Task UpdateAsync_EmptyName_ThrowsValidationException()
    {
        var dto = new UpdateProductDto { Name = "", Price = 10m };

        Func<Task> act = () => _service.UpdateAsync(Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*name cannot be empty*");
    }

    [Fact]
    public async Task UpdateAsync_WhitespaceName_ThrowsValidationException()
    {
        var dto = new UpdateProductDto { Name = "  ", Price = 10m };

        Func<Task> act = () => _service.UpdateAsync(Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*name cannot be empty*");
    }

    [Fact]
    public async Task UpdateAsync_NegativePrice_ThrowsValidationException()
    {
        var dto = new UpdateProductDto { Name = "Valid", Price = -5m };

        Func<Task> act = () => _service.UpdateAsync(Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*price cannot be negative*");
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ThrowsProductNotFoundException()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateProductDto { Name = "Valid", Price = 10m };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

        Func<Task> act = () => _service.UpdateAsync(id, dto);

        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_CallsDelete()
    {
        var product = new Product("To Delete", "Desc", 1.0m);
        _repoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _repoMock.Setup(r => r.DeleteAsync(product.Id)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(product.Id);

        _repoMock.Verify(r => r.DeleteAsync(product.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ThrowsProductNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

        Func<Task> act = () => _service.DeleteAsync(id);

        await act.Should().ThrowAsync<ProductNotFoundException>();
    }
}
