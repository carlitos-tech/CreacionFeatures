using CreacionFeatures.Application.DTOs;

namespace CreacionFeatures.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<ProductDto> CreateAsync(CreateProductDto createDto);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateDto);
    Task DeleteAsync(Guid id);
}
