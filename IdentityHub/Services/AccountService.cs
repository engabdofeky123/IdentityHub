using IdentityHub.Helpers;
using IdentityHub.Interfaces;
using IdentityHub.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityHub.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly FrontendOptions _frontendOptions;

        public AccountService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, 
            IEmailService emailService, FrontendOptions frontendOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _frontendOptions = frontendOptions;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;

        }
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(email);

            var resetLink = $"{_frontendOptions.BaseUrl}?email={encodedEmail}&token={encodedToken}";

            var subject = "Reset Your Password";
            var body = $"""
            <h2>Password Reset</h2>
            <p>We received a request to reset your password.</p>
            <p>
                <a href="{resetLink}">
                    Reset Password
                </a>
            </p>
            <p>If you didn't request this, ignore this email.</p>
            """;

            await _emailService.SendEmailAsync("abdofeky123@gmail.com", email, subject, body);

        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> SendEmailConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || user.EmailConfirmed)
                return false;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var encodedUserId = Uri.EscapeDataString(user.Id);

            var confirmationLink =
                $"{_frontendOptions.BaseUrl}" +
                $"{_frontendOptions.ConfirmEmailPath}" +
                $"?userId={encodedUserId}&token={encodedToken}";

            var subject = "Confirm Your Email";

            var body = $"""
                <h2>Email Confirmation</h2>
                <p>Please confirm your email address.</p>
                <a href="{confirmationLink}">Confirm Email</a>
                """;

            await _emailService.SendEmailAsync("nabta2027@gmail.com", user.Email!, subject, body);

            return true;
        }
    }
}
