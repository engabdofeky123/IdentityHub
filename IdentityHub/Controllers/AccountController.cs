using IdentityHub.DTOs.Account;
using IdentityHub.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            await _accountService.SendEmailConfirmationAsync(email);
            return Ok("Confirmation email sent.");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("User ID and token are required.");

            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (result)
                return Ok("Email confirmed successfully.");
            else
                return BadRequest("Invalid token or user ID.");
        }

        [HttpPost("send-reset-password-email")]
        public async Task<IActionResult> SendResetPasswordEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            await _accountService.ForgotPasswordAsync(email);
            return Ok("Reset password email sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.ResetPasswordAsync(model.Email,model.Token,model.NewPassword);

            if (!result)
                return BadRequest("Invalid or expired token.");

            return Ok("Password reset successfully.");
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var result = await _accountService.SendEmailConfirmationAsync(email);

            if (!result)
                return BadRequest("Unable to resend confirmation email.");

            return Ok("Confirmation email resent successfully.");
        }
    }
}