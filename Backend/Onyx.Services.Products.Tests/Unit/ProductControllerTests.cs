using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Onyx.Common;
using Onyx.Services.Products.Controllers;
using Onyx.Services.Products.Domain.Requests;
using Onyx.Services.Products.Domain.Resources;
using Onyx.Services.Products.Messaging.Commands;
using Onyx.Services.Products.Store;
using Rebus.Bus;

namespace Onyx.Services.Products.Tests.Unit;

public class ProductControllerTests
{
    private readonly Mock<IBus> _busMock;
    private readonly Mock<IProductsRepository> _repositoryMock;
    private readonly ProductsController _sut;

    public ProductControllerTests()
    {
        _busMock = new Mock<IBus>();
        _repositoryMock = new Mock<IProductsRepository>();

        _sut = new ProductsController(_busMock.Object, _repositoryMock.Object);
    }
    
    [Fact]
    public async Task Get_CallsRepository_ReturnsAction()
    {
        // Arrange
        const string filter = "green";
        var product = new Product(Guid.NewGuid(), "BMW", "M4", 5500090, filter);

        _repositoryMock
            .Setup(s => s.GetProductsAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok<IEnumerable<Product>>(new []
            {
                product
            }));
        
        // Act
        var result = await _sut.Get(filter, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<Product>>>();
        result.Value?.Should().ContainSingle(s => s.Equals(product));
    }
    
    [Fact]
    public async Task Post_CreatesProduct_CallsBus_ReturnsAccepted()
    {
        // Arrange
        var request = new CreateProductRequest("BMW", "M2", 15000, "Blue");

        _busMock
            .Setup(s => s.Send(It.IsAny<CreateProductCommand>(), null));
        
        // Act
        var result = await _sut.Post(request);

        // Assert
        result.Should().BeOfType<ActionResult<AsyncApiResponse>>();
        
        _busMock.Verify(x => x.Send(It.Is<CreateProductCommand>(p => 
                p.Product.Name.Equals(request.Name) && 
                p.Product.Description.Equals(request.Description) &&
                p.Product.PriceInMinorUnits.Equals(request.PriceInMinorUnits) &&
                p.Product.Colour.Equals(request.Colour)
            ), null), Times.Once);
    }
}