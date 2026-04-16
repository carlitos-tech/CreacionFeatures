namespace CreacionFeatures.Domain.Exceptions;

public class ProductNotFoundException : DomainException
{
    public Guid ProductId { get; }

    public ProductNotFoundException(Guid id)
        : base($"Product with id '{id}' was not found.")
    {
        ProductId = id;
    }
}
