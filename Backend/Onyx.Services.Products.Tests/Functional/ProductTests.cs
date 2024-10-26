using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Onyx.Services.Products.Domain.Resources;
using Onyx.Services.Products.Messaging.Commands;
using Onyx.Services.Products.Store;

namespace Onyx.Services.Products.Tests.Functional;

public class ProductTests(GenericTestFixture fixture) : IClassFixture<GenericTestFixture>
{
    [Fact]
    public async Task SendCreateProductCommand_SavesToDatabase()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product(productId, "BMW M2", "Best coupe in the world", 5500000, "White");
        var command = new CreateProductCommand(product);

        // Act
        await fixture.Bus.Send(command);
        
        //Note: This is a dirty hack to wait for the queue to process.
        //A Rebus test harness should be included to count the pending json files and then complete the processing task when done. 
        await Task.Delay(500); 
        
        // Assert
        var dbProduct = await fixture.DbContext.Products.FindAsync(productId);

        dbProduct.Should().NotBeNull();

        dbProduct!.ProductId.Should().Be(product.ProductId);
        dbProduct.Name.Should().Be(product.Name);
        dbProduct.Description.Should().Be(product.Description);
        dbProduct.Colour.Should().Be(product.Colour);
        dbProduct.PriceInMinorUnits.Should().Be(product.PriceInMinorUnits);
    }

    [Theory]
    [MemberData(nameof(FilterTestData))]
    public async Task GetProducts_FiltersItems(string? filter, int expectedCount)
    {
        var repo = fixture.Services.GetRequiredService<IProductsRepository>();

        var productsResult = await repo.GetProductsAsync(filter, CancellationToken.None);

        productsResult.IsSuccess.Should().BeTrue();

        var products = productsResult.Value;

        products.Count().Should().Be(expectedCount);
    }
    
    public static IEnumerable<object[]> FilterTestData = new List<object[]>
    {
        new object[] { "Black", 0 },
        new object[] { "red", 2 }, // check for case sensitivity
        new object[] { "Red", 2 },
        new object[] { "Green", 1 },
        new object[] { "Blue", 1 },
    };
}