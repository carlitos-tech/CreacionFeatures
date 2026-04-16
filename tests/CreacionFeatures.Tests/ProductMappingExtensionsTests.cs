using CreacionFeatures.Application.Mappings;
using CreacionFeatures.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CreacionFeatures.Tests;

public class ProductMappingExtensionsTests
{
    [Fact]
    public void ToDto_MapsAllProperties()
    {
        var product = new Product("Mapped", "Mapped Desc", 42.5m);

        var dto = product.ToDto();

        dto.Id.Should().Be(product.Id);
        dto.Name.Should().Be(product.Name);
        dto.Description.Should().Be(product.Description);
        dto.Price.Should().Be(product.Price);
        dto.CreatedAt.Should().Be(product.CreatedAt);
    }
}
