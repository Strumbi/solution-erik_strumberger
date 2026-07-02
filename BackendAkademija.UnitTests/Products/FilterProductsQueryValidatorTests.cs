using BackendAkademija.Application.Products.Queries.FilterProducts;
using FluentAssertions;

namespace BackendAkademija.UnitTests.Products;

public class FilterProductsQueryValidatorTests
{
    private readonly FilterProductsQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldPass_WhenAllParametersValid()
    {
        var query = new FilterProductsQuery("smartphones", 100, 500);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenNoParametersProvided()
    {
        var query = new FilterProductsQuery(null, null, null);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenMinPriceGreaterThanMaxPrice()
    {
        var query = new FilterProductsQuery(null, 500, 100);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.ErrorMessage == "MinPrice mora biti manji ili jednak MaxPrice.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenMinPriceIsNegative()
    {
        var query = new FilterProductsQuery(null, -1, null);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenMaxPriceIsNegative()
    {
        var query = new FilterProductsQuery(null, null, -1);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenMinAndMaxPriceAreEqual()
    {
        var query = new FilterProductsQuery(null, 100, 100);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }
}