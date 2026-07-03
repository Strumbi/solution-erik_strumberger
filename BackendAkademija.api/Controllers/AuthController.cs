using BackendAkademija.Application.Dto.AuthDto;
using BackendAkademija.Application.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackendAkademija.api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(loginRequestDto.Username, loginRequestDto.Password, cancellationToken);
        
        if(!result.Success) return Unauthorized(new {result.Error});
        
        return Ok(new
        {
            result.AccessToken,
            result.RefreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(request.RefreshToken, cancellationToken);
        
        if(!result.Success) return Unauthorized(new {result.Error});
        
        return Ok(new
        {
            result.AccessToken,
            result.RefreshToken
        });
    }
}