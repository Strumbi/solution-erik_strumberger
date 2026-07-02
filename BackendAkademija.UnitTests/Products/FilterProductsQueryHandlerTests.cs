using BackendAkademija.Application.Interfaces;
using BackendAkademija.Application.Products.Queries.FilterProducts;
using BackendAkademija.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BackendAkademija.UnitTests.Products;

public class FilterProductsQueryHandlerTests
{
    private readonly Mock<IProductSource> _productsSourceMock;
    private readonly FilterProductsQueryHandler _handler;

    public FilterProductsQueryHandlerTests()
    {
        _productsSourceMock = new Mock<IProductSource>();
        _handler = new FilterProductsQueryHandler(_productsSourceMock.Object);
    }

    private List<Product> GetTestProducts() => new()
    {
        new() { Id = 1, Title = "iPhone", Price = 999, Category = "smartphones" },
        new() { Id = 2, Title = "Samsung", Price = 799, Category = "smartphones" },
        new() { Id = 3, Title = "MacBook", Price = 1999, Category = "laptops" },
        new() { Id = 4, Title = "Dell XPS", Price = 1499, Category = "laptops" }
    };

    [Fact]
    public async Task Handle_ShouldFilterByCategory()
    {
        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetTestProducts());

        var result = await _handler.Handle(
            new FilterProductsQuery("smartphones", null, null), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Title == "iPhone" || p.Title == "Samsung");
    }

    [Fact]
    public async Task Handle_ShouldFilterByMinPrice()
    {
        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetTestProducts());

        var result = await _handler.Handle(
            new FilterProductsQuery(null, 1000, null), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Title == "MacBook" || p.Title == "Dell XPS");
    }

    [Fact]
    public async Task Handle_ShouldFilterByMaxPrice()
    {
        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetTestProducts());

        var result = await _handler.Handle(
            new FilterProductsQuery(null, null, 800), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Samsung");
    }

    [Fact]
    public async Task Handle_ShouldFilterByCategoryAndPriceRange()
    {
        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetTestProducts());

        var result = await _handler.Handle(
            new FilterProductsQuery("laptops", 1500, 2000), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Title.Should().Be("MacBook");
    }

    [Fact]
    public async Task Handle_ShouldReturnAll_WhenNoFiltersApplied()
    {
        _productsSourceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetTestProducts());

        var result = await _handler.Handle(
            new FilterProductsQuery(null, null, null), CancellationToken.None);

        result.Should().HaveCount(4);
    }
}