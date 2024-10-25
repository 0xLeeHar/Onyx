using Onyx.Services.Products.Domain.Resources;

namespace Onyx.Services.Products.Messaging.Commands;

/// <summary>
/// Creates a new Product.
/// </summary>
/// <param name="Product">The new product to create</param>
public record CreateProductCommand(Product Product);