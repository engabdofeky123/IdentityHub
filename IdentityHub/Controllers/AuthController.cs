using IdentityHub.DTOs.Auth;
using IdentityHub.Interfaces;
using IdentityHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        public AuthController(IAuthService auth, IConfiguration config)
        {
            _authService = auth;
            _config = config;
        }

        // Standard email/password authentication endpoints

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result.IsAuthenticated == false || result is null)
                return Unauthorized("Invalid email or password");
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result.IsAuthenticated == false || result is null)
                return BadRequest("User with this email already exists");
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var me = await _authService.GetMe(userId);
            return Ok(me);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.Token);
            return Ok(result);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshToken dto)
        {
            var result = await _authService.RevokeToken(dto.Token);
            if (!result)
                return BadRequest("Invalid token");
            return Ok(new { message = "Token revoked" });
        }
    }
}
