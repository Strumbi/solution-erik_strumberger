using BackendAkademija.Application.Interfaces;
using BackendAkademija.Application.Products.Queries.GetProductById;
using BackendAkademija.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BackendAkademija.UnitTests.Products;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductSource> _productsSourceMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _productsSourceMock = new Mock<IProductSource>();
        _handler = new GetProductByIdQueryHandler(_productsSourceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            Id = 1, Title = "Phone", Price = 999,
            Description = "Nice phone", Category = "smartphones"
        };

        _productsSourceMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(new GetProductByIdQuery(1), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Phone");
        result.Category.Should().Be("smartphones");
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        _productsSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.Handle(new GetProductByIdQuery(999), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}