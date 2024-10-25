using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Onyx.Services.Products.Domain.Resources;
using Onyx.Services.Products.Extensions;
using Onyx.Services.Products.Messaging.Commands;
using Onyx.Services.Products.Store;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;

namespace Onyx.Services.Products.Tests.Functional;

public class GenericTestFixture : IAsyncLifetime
{
    public IServiceProvider Services { get; init; }
    public ProductsContext DbContext { get; init; }
    public IBus Bus { get; init; }

    public GenericTestFixture()
    {
        // Services
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddProductsService();

        serviceCollection.AddDbContext<ProductsContext>(b =>
        {
            b.UseInMemoryDatabase("Tests");
        });
        
        const string queueName = "onyx-queue";
        serviceCollection.AddRebus(config =>
            config
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), queueName))
                .Logging(l => l.ColoredConsole())
                .Routing(cf =>
                {
                    cf.TypeBased()
                        .MapAssemblyOf<CreateProductCommand>(queueName);
                })
        );

        Services = serviceCollection.BuildServiceProvider();

        DbContext = Services.GetRequiredService<ProductsContext>();
        Bus = Services.GetRequiredService<IBus>();
    }

    public async Task InitializeAsync()
    {
        await SeedTestDataAsync();
    }

    public Task DisposeAsync()
    {
        // No-op
        return Task.CompletedTask;
    }
    
    private async Task SeedTestDataAsync()
    {
        await DbContext.Products.AddRangeAsync(new[]
        {
            new Product(Guid.NewGuid(), "Fast red car", "Ferrari, zoom zoom", 1000, "Red"),
            new Product(Guid.NewGuid(), "Slow red car", "Not a ferrari", 20, "Red"),
            new Product(Guid.NewGuid(), "Van", "Moves stuff", 50, "Green"),
            new Product(Guid.NewGuid(), "Ford", "Fix or repair daily", 50, "Blue")
        });

        await DbContext.SaveChangesAsync();
    }
}