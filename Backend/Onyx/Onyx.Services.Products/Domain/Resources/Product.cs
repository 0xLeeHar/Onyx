namespace Onyx.Services.Products.Domain.Resources;

/// <summary>
/// Domain model for a product.
/// </summary>
public record Product(
    Guid ProductId,
    string Name, 
    string Description,
    int PriceInMinorUnits,
    string Colour);