using FluentResults;
using Microsoft.EntityFrameworkCore;
using Onyx.Services.Products.Domain.Resources;

namespace Onyx.Services.Products.Store;

public interface IProductsRepository
{
    /// <summary>
    /// Gets a collection of Product
    /// </summary>
    /// <param name="colour">A colour to filter by (optional)</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task<Result<IEnumerable<Product>>> GetProductsAsync(string? colour, CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new product in the data store
    /// </summary>
    /// <param name="product">The product to create</param>
    /// <returns>A result of the new Product</returns>
    Task<Result<Product>> CreateProductAsync(Product product);
}

public class ProductsRepository(ProductsContext context) : IProductsRepository
{
    public async Task<Result<IEnumerable<Product>>> GetProductsAsync(string? colour, CancellationToken cancellationToken)
    {
        try
        {
            var result = await context
                .Products
                .Where(w => string.IsNullOrWhiteSpace(colour) 
                            || w.Colour.Equals(colour, StringComparison.InvariantCultureIgnoreCase))
                .ToListAsync(cancellationToken);
            
            return result;
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }

    public async Task<Result<Product>> CreateProductAsync(Product product)
    {
        try
        {
            await context.AddAsync(product);
            await context.SaveChangesAsync();

            return product;
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }
}