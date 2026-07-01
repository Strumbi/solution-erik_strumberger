using BackendAkademija.Application.Dto.AuthDto;
using BackendAkademija.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendAkademija.api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(loginRequestDto.Username, loginRequestDto.Password, cancellationToken);
        
        if(!result.Succeeded) return Unauthorized(new {result.Error});
        
        return Ok(new {result});
    }
}