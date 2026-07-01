using BackendAkademija.Application.Products.Queries;
using BackendAkademija.Application.Products.Queries.FilterProducts;
using BackendAkademija.Application.Products.Queries.GetProductById;
using BackendAkademija.Application.Products.Queries.SearchProductsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAkademija.api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var resutl = await mediator.Send(new GetProductsListQuery(), cancellationToken);
        return Ok(resutl);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        
        if(result is null) return NotFound(new { Message = $"Product wtih ID {id} is not efund." });
        
        return Ok(result);
    }
    
    [HttpGet("filter")]
    public async Task<IActionResult> FilterProducts(
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new FilterProductsQuery(category, minPrice, maxPrice), cancellationToken);
        return Ok(result);
    }
    
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string searchTerm,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(new { Message = "Search term shoul not be empyt" });

        var result = await mediator.Send(new SearchProductsQuery(searchTerm), ct);
        return Ok(result);
    }
}