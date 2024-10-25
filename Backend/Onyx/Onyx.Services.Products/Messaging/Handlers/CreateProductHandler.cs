using Onyx.Common;
using Onyx.Common.Errors;
using Onyx.Services.Products.Domain.Resources;
using Onyx.Services.Products.Messaging.Commands;
using Onyx.Services.Products.Store;
using Rebus.Handlers;

namespace Onyx.Services.Products.Messaging.Handlers;

/// <summary>
/// Handles the creation of a new product
/// </summary>
public class CreateProductHandler(IProductsRepository repository) : IHandleMessages<CreateProductCommand>
{
    public async Task Handle(CreateProductCommand message)
    {
        // TODO: There should be more business logic here
        
        var result = await repository.CreateProductAsync(message.Product);

        if (result.IsFailed)
        {
            //Note: We want to throw here so the message is added to the dead-letter queue
            throw new ErrorException<Product>(result);
        }
        
        // Note: Could raise `ProductCreatedEvent` here if needed.
    }
}