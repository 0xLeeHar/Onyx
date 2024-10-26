using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Onyx.Services.Products.Domain.Requests;
using Onyx.Services.Products.Messaging.Handlers;
using Onyx.Services.Products.Store;
using Rebus.Config;

namespace Onyx.Services.Products.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the products "micro-service" to the host applications service collection.
    /// </summary>
    /// <param name="services">The service collection of the host.</param>
    public static void AddProductsService(this IServiceCollection services)
    {
        services.AddTransient<IProductsRepository, ProductsRepository>();

        // Database context
        services.AddDbContext<ProductsContext>(opts =>
        {
            //TODO: This is an in-memory DB and should be hosted in the real world.
            opts.UseInMemoryDatabase("Onyx-OB");
        });
        
        // Health 
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthChecks>("Products-Database");
        
        // Validation
        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
        
        // Handlers
        services.AddTransient<CreateProductHandler>();
        services.AddRebusHandler<CreateProductHandler>();
    }   
}