using Microsoft.AspNetCore.Mvc;
using Platform.Application.Interfaces;
using Platform.Domain.Exceptions;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        try
        {
            await _authService.RegisterAsync(request.Email, request.Password);
            return Ok(new { message = "Usuario registrado exitosamente." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        try
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(new { token });
        }
        catch (DomainException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}

public record AuthRequest(string Email, string Password);
