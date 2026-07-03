using BackendAkademija.Application.Products.Queries.GetCatrgories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAkademija.api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(result);
    }
}