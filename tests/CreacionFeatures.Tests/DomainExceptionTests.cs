using CreacionFeatures.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CreacionFeatures.Tests;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        var ex = new DomainException("test error");
        ex.Message.Should().Be("test error");
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsBoth()
    {
        var inner = new Exception("inner");
        var ex = new DomainException("outer", inner);
        ex.Message.Should().Be("outer");
        ex.InnerException.Should().Be(inner);
    }
}
