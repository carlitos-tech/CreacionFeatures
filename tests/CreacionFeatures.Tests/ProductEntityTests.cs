using CreacionFeatures.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CreacionFeatures.Tests;

public class ProductEntityTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var before = DateTime.UtcNow;
        var product = new Product("Name", "Description", 99.99m);
        var after = DateTime.UtcNow;

        product.Id.Should().NotBe(Guid.Empty);
        product.Name.Should().Be("Name");
        product.Description.Should().Be("Description");
        product.Price.Should().Be(99.99m);
        product.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Update_ChangesNameDescriptionAndPrice()
    {
        var product = new Product("Old", "Old Desc", 1.0m);

        product.Update("New", "New Desc", 2.0m);

        product.Name.Should().Be("New");
        product.Description.Should().Be("New Desc");
        product.Price.Should().Be(2.0m);
    }
}
