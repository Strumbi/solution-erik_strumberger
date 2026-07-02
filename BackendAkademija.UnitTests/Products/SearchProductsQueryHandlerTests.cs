using BackendAkademija.Application.Interfaces;
using BackendAkademija.Application.Products.Queries.SearchProductsQuery;
using BackendAkademija.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BackendAkademija.UnitTests.Products;

public class SearchProductsQueryHandlerTests
{
    private readonly Mock<IProductSource> _productsSourceMock;
    private readonly SearchProductsQueryHandler _handler;

    public SearchProductsQueryHandlerTests()
    {
        _productsSourceMock = new Mock<IProductSource>();
        _handler = new SearchProductsQueryHandler(_productsSourceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMatchingProducts_WhenSearchTermMatches()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Title = "iPhone 14", Price = 999 },
            new() { Id = 2, Title = "Samsung Galaxy", Price = 799 },
            new() { Id = 3, Title = "iPhone 15", Price = 1099 }
        };

        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(
            new SearchProductsQuery("iphone"), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Title.Contains("iPhone"));
    }

    [Fact]
    public async Task Handle_ShouldBeCaseInsensitive()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Title = "iPhone 14", Price = 999 }
        };

        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var resultLower = await _handler.Handle(
            new SearchProductsQuery("iphone"), CancellationToken.None);
        var resultUpper = await _handler.Handle(
            new SearchProductsQuery("IPHONE"), CancellationToken.None);

        // Assert
        resultLower.Should().HaveCount(1);
        resultUpper.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoProductsMatch()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Title = "Samsung Galaxy", Price = 799 }
        };

        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(
            new SearchProductsQuery("iphone"), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}