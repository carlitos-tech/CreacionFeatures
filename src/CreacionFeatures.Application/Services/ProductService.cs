using CreacionFeatures.Application.DTOs;
using CreacionFeatures.Application.Interfaces;
using CreacionFeatures.Application.Mappings;
using CreacionFeatures.Domain.Entities;
using CreacionFeatures.Domain.Exceptions;
using CreacionFeatures.Domain.Interfaces;

namespace CreacionFeatures.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => p.ToDto());
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null)
            throw new ProductNotFoundException(id);
        return product.ToDto();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.Name))
            throw new ValidationException("Product name cannot be empty.");
        if (createDto.Price < 0)
            throw new ValidationException("Product price cannot be negative.");

        var product = new Product(createDto.Name, createDto.Description, createDto.Price);
        var created = await _repository.AddAsync(product);
        return created.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateDto)
    {
        if (string.IsNullOrWhiteSpace(updateDto.Name))
            throw new ValidationException("Product name cannot be empty.");
        if (updateDto.Price < 0)
            throw new ValidationException("Product price cannot be negative.");

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new ProductNotFoundException(id);

        existing.Update(updateDto.Name, updateDto.Description, updateDto.Price);
        var updated = await _repository.UpdateAsync(existing);
        return updated.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new ProductNotFoundException(id);
        await _repository.DeleteAsync(id);
    }
}
