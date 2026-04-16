using CreacionFeatures.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CreacionFeatures.Tests;

public class ProductNotFoundExceptionTests
{
    [Fact]
    public void Constructor_SetsProductId()
    {
        var id = Guid.NewGuid();
        var ex = new ProductNotFoundException(id);

        ex.ProductId.Should().Be(id);
    }

    [Fact]
    public void Constructor_SetsMessage_ContainingId()
    {
        var id = Guid.NewGuid();
        var ex = new ProductNotFoundException(id);

        ex.Message.Should().Contain(id.ToString());
    }

    [Fact]
    public void IsAssignableTo_DomainException()
    {
        var id = Guid.NewGuid();
        var ex = new ProductNotFoundException(id);

        ex.Should().BeAssignableTo<DomainException>();
    }
}

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_SetsMessage()
    {
        var ex = new ValidationException("field is required");

        ex.Message.Should().Be("field is required");
    }

    [Fact]
    public void IsAssignableTo_DomainException()
    {
        var ex = new ValidationException("error");

        ex.Should().BeAssignableTo<DomainException>();
    }
}
