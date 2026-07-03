using BackendAkademija.Application.Products.Queries.GetCatrgories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAkademija.api.Controllers;

/// <summary>
/// Endpointi za dohvat kategorija proizvoda.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Dohvaća listu svih kategorija.
    /// </summary>
    /// <param name="cancellationToken">Token za otkazivanje asinkrone operacije.</param>
    /// <returns>Lista kategorija.</returns>
    /// <response code="200">Lista kategorija uspješno dohvaćena.</response>
    /// <response code="401">Korisnik nije autoriziran.</response>
    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(result);
    }
}