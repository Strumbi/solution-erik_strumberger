using BackendAkademija.Application.Products.Queries;
using BackendAkademija.Application.Products.Queries.FilterProducts;
using BackendAkademija.Application.Products.Queries.GetProductById;
using BackendAkademija.Application.Products.Queries.SearchProductsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAkademija.api.Controllers;

/// <summary>
/// Endpointi za dohvat, pretragu i filtriranje proizvoda.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Dohvaća listu svih proizvoda.
    /// </summary>
    /// <param name="cancellationToken">Token za otkazivanje asinkrone operacije.</param>
    /// <returns>Lista svih proizvoda.</returns>
    /// <response code="200">Lista proizvoda uspješno dohvaćena.</response>
    /// <response code="401">Korisnik nije autoriziran.</response>
    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var resutl = await mediator.Send(new GetProductsListQuery(), cancellationToken);
        return Ok(resutl);
    }

    /// <summary>
    /// Dohvaća proizvod prema ID-u.
    /// </summary>
    /// <param name="id">ID proizvoda koji se dohvaća.</param>
    /// <param name="cancellationToken">Token za otkazivanje asinkrone operacije.</param>
    /// <returns>Podaci o traženom proizvodu.</returns>
    /// <response code="200">Proizvod pronađen i vraćen.</response>
    /// <response code="401">Korisnik nije autoriziran.</response>
    /// <response code="404">Proizvod s danim ID-em ne postoji.</response>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        
        if(result is null) return NotFound(new { Message = $"Product wtih ID {id} is not efund." });
        
        return Ok(result);
    }
    
    
    /// <summary>
    /// Filtrira proizvode prema kategoriji i/ili rasponu cijena.
    /// </summary>
    /// <param name="category">Naziv kategorije po kojoj se filtrira (opcionalno).</param>
    /// <param name="minPrice">Minimalna cijena proizvoda (opcionalno).</param>
    /// <param name="maxPrice">Maksimalna cijena proizvoda (opcionalno).</param>
    /// <param name="cancellationToken">Token za otkazivanje asinkrone operacije.</param>
    /// <returns>Lista proizvoda koji zadovoljavaju zadane filtere.</returns>
    /// <response code="200">Filtrirana lista proizvoda uspješno vraćena.</response>
    /// <response code="401">Korisnik nije autoriziran.</response>
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
    
    /// <summary>
    /// Pretražuje proizvode prema zadanom pojmu za pretragu.
    /// </summary>
    /// <param name="searchTerm">Pojam po kojem se pretražuju proizvodi (obavezan).</param>
    /// <param name="ct">Token za otkazivanje asinkrone operacije.</param>
    /// <returns>Lista proizvoda koji odgovaraju pojmu za pretragu.</returns>
    /// <response code="200">Rezultati pretrage uspješno vraćeni.</response>
    /// <response code="400">Pojam za pretragu nije zadan ili je prazan.</response>
    /// <response code="401">Korisnik nije autoriziran.</response>
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