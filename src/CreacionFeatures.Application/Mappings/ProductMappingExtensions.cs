using CreacionFeatures.Application.DTOs;
using CreacionFeatures.Domain.Entities;

namespace CreacionFeatures.Application.Mappings;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        };
    }
}
