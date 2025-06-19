using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditApp.Application.DTOs;
using CreditApp.Application.Services;
using System.Security.Claims;
using MediatR;

namespace CreditApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, [FromServices] IMediator mediator)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var command = new LoginCommand(request.Email, request.Password, ip);
                var response = await mediator.Send(command);
                return Ok(response);
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Credenciales inválidas o error de autenticación" });
            }
        }

        [Authorize]
        [HttpGet("whoami")]
        public async Task<ActionResult<AuthResponse>> WhoAmI()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });
                var userId = Guid.Parse(userIdClaim);
                var response = await _authService.GetCurrentUserAsync(userId);
                return Ok(response);
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, [FromServices] IMediator mediator)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var command = new RegisterCommand(request.Username, request.Email, request.Password, ip);
                var response = await mediator.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register-analyst")]
        public async Task<ActionResult<AuthResponse>> RegisterAnalyst(RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAnalystAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public record UpdatePasswordRequest(string CurrentPassword, string NewPassword);
    public record UpdateEmailRequest(string NewEmail);
} 