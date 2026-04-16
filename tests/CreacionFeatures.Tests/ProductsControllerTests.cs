using CreacionFeatures.API.Controllers;
using CreacionFeatures.Application.DTOs;
using CreacionFeatures.Application.Interfaces;
using CreacionFeatures.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CreacionFeatures.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _controller = new ProductsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        var products = new List<ProductDto> { new() { Id = Guid.NewGuid(), Name = "P1" } };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var dto = new ProductDto { Id = id, Name = "Found" };
        _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dto);

        var result = await _controller.GetById(id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new DomainException("Product with id 'xxx' was not found."));

        var result = await _controller.GetById(Guid.NewGuid());

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_ValidDto_ReturnsCreatedAtAction()
    {
        var dto = new CreateProductDto { Name = "New", Description = "Desc", Price = 10m };
        var created = new ProductDto { Id = Guid.NewGuid(), Name = "New", Price = 10m };
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Create_InvalidDto_ReturnsBadRequest()
    {
        var dto = new CreateProductDto { Name = "" };
        _serviceMock.Setup(s => s.CreateAsync(dto))
            .ThrowsAsync(new DomainException("Product name cannot be empty."));

        var result = await _controller.Create(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ValidDto_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateProductDto { Name = "Updated", Price = 20m };
        var updated = new ProductDto { Id = id, Name = "Updated", Price = 20m };
        _serviceMock.Setup(s => s.UpdateAsync(id, dto)).ReturnsAsync(updated);

        var result = await _controller.Update(id, dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_NotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateProductDto { Name = "Updated", Price = 10m };
        _serviceMock.Setup(s => s.UpdateAsync(id, dto))
            .ThrowsAsync(new DomainException($"Product with id '{id}' was not found."));

        var result = await _controller.Update(id, dto);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_InvalidDto_ReturnsBadRequest()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateProductDto { Name = "", Price = 10m };
        _serviceMock.Setup(s => s.UpdateAsync(id, dto))
            .ThrowsAsync(new DomainException("Product name cannot be empty."));

        var result = await _controller.Update(id, dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(id);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_NotFound_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new DomainException("Product with id 'xxx' was not found."));

        var result = await _controller.Delete(Guid.NewGuid());

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
