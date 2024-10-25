using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onyx.Common;
using Onyx.Common.Extensions;
using Onyx.Services.Products.Domain.Requests;
using Onyx.Services.Products.Domain.Resources;
using Onyx.Services.Products.Messaging.Commands;
using Onyx.Services.Products.Store;
using Rebus.Bus;

namespace Onyx.Services.Products.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "APIKEY")]
[Produces("application/json")]
[Route("api/[controller]")]
public class ProductsController(IBus bus, IProductsRepository repository) : ControllerBase
{
    /// <summary>
    /// Gets a collection of products based on the filter.
    /// </summary>
    /// <param name="colour">
    /// The colour of the products to filter by.
    /// Consider using OData filtering onto the DB Context for a better experience.
    /// </param>
    /// <param name="cancellationToken">A cancellation token from the HTTP middleware</param>
    /// <returns>A collection of Product</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get(string? colour, CancellationToken cancellationToken)
    {
        var result = await repository.GetProductsAsync(colour, cancellationToken);
        
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">The details of the product to be created.</param>
    /// <returns>
    /// This is an async API, a resource ID will be returned but the product will be created some time in the future.
    /// The calling code will then need to call the GET api to check when its been created.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<AsyncApiResponse>> Post(CreateProductRequest request)
    {   
        var resourceId = Guid.NewGuid(); // TODO: This should come from some ID creation service.

        // TODO: Could use Mapster/AutoMapper here.
        var newProduct = new Product(resourceId, request.Name, request.Description, request.PriceInMinorUnits, request.Colour); 
        
        await bus.Send(new CreateProductCommand(newProduct));

        return Accepted(new AsyncApiResponse(resourceId));
    }
}