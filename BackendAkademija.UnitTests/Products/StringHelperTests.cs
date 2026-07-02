using BackendAkademija.Application.Helper;
using FluentAssertions;

namespace BackendAkademija.UnitTests.Products;

public class StringHelperTests
{
    [Fact]
    public void Truncate_ShouldReturnOriginal_WhenTextShorterThanMaxLength()
    {
        var result = StringHelper.Truncate("Hello", 100);
        result.Should().Be("Hello");
    }

    [Fact]
    public void Truncate_ShouldTruncateWithEllipsis_WhenTextExceedsMaxLength()
    {
        var longText = new string('a', 150);
        var result = StringHelper.Truncate(longText, 100);
        result.Should().EndWith("...");
        result.Length.Should().Be(103);
    }

    [Fact]
    public void Truncate_ShouldReturnOriginal_WhenTextEqualsMaxLength()
    {
        var text = new string('a', 100);
        var result = StringHelper.Truncate(text, 100);
        result.Should().Be(text);
    }

    [Fact]
    public void Truncate_ShouldHandleEmptyString()
    {
        var result = StringHelper.Truncate(string.Empty, 100);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Truncate_ShouldHandleNullString()
    {
        var result = StringHelper.Truncate(null!, 100);
        result.Should().BeEmpty();
    }
}