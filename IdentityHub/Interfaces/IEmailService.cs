namespace IdentityHub.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string from, string to, string subject, string msg);
    }
}