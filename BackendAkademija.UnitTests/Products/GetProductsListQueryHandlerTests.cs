using BackendAkademija.Application.Interfaces;
using BackendAkademija.Application.Products.Queries;
using BackendAkademija.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BackendAkademija.UnitTests.Products;

public class GetProductsListQueryHandlerTests
{
    private readonly Mock<IProductSource> _productsSourceMock;
    private readonly GetProductsListQueryHandler _handler;

    public GetProductsListQueryHandlerTests()
    {
        _productsSourceMock = new Mock<IProductSource>();
        _handler = new GetProductsListQueryHandler(_productsSourceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductList_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Title = "Phone", Price = 999, Description = "Nice phone", Thumbnail = "img.jpg" },
            new() { Id = 2, Title = "Laptop", Price = 1999, Description = "Nice laptop", Thumbnail = "img2.jpg" }
        };

        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(new GetProductsListQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Phone");
        result[0].Price.Should().Be(999);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsExist()
    {
        // Arrange
        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await _handler.Handle(new GetProductsListQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldTruncateDescription_WhenDescriptionExceeds100Chars()
    {
        // Arrange
        var longDescription = new string('a', 150);
        var products = new List<Product>
        {
            new() { Id = 1, Title = "Phone", Price = 999, Description = longDescription }
        };

        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(new GetProductsListQuery(), CancellationToken.None);

        // Assert
        result[0].ShortDescription.Length.Should().BeLessOrEqualTo(103); // 100 + "..."
    }
}