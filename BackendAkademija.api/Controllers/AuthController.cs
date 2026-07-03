using BackendAkademija.Application.Dto.AuthDto;
using BackendAkademija.Application.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackendAkademija.api.Controllers;

/// <summary>
/// Endpointi za autentikaciju korisnika (prijava i osvježavanje tokena).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{

    /// <summary>
    /// Prijavljuje korisnika na temelju korisničkog imena i lozinke.
    /// </summary>
    /// <param name="loginRequestDto">Korisničko ime i lozinka korisnika.</param>
    /// <param name="cancellationToken">Token za otkazivanje asinkrone operacije.</param>
    /// <returns>Access i refresh token ukoliko je prijava uspješna.</returns>
    /// <response code="200">Prijava uspješna, vraća access i refresh token.</response>
    /// <response code="401">Neispravno korisničko ime ili lozinka.</response>
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

    
    /// <summary>
    /// Izdaje novi par access/refresh tokena na temelju postojećeg refresh tokena.
    /// </summary>
    /// <param name="request">Zahtjev koji sadrži trenutni refresh token.</param>
    /// <param name="cancellationToken">Token za otkazivanje asinkrone operacije.</param>
    /// <returns>Novi access i refresh token.</returns>
    /// <response code="200">Token uspješno osvježen.</response>
    /// <response code="401">Refresh token je nevažeći ili je istekao.</response>
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