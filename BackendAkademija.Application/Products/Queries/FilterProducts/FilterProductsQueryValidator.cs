using FluentValidation;

namespace BackendAkademija.Application.Products.Queries.FilterProducts;

public class FilterProductsQueryValidator : AbstractValidator<FilterProductsQuery>
{
    public FilterProductsQueryValidator()
    {
        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue);
        
        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue);
        
        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
            .WithMessage("MinPrice mora biti manji ili jednak MaxPrice.");

    }
    
}