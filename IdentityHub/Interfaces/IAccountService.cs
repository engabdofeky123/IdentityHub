namespace IdentityHub.Interfaces
{
    public interface IAccountService
    {
        Task<bool> ConfirmEmailAsync(string userId, string token);

        Task ForgotPasswordAsync(string email);

        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

        Task<bool> SendEmailConfirmationAsync(string email);
    }
}
