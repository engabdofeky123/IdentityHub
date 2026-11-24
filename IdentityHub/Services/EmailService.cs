using IdentityHub.Interfaces;
using System.Net;
using System.Net.Mail;

namespace IdentityHub.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string from, string to, string subject, string msg)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("nabta2027@gmail.com", "qqarwwkcbuziybcv"),
                EnableSsl = true
            };
            var mailMessage = new MailMessage()
            {
                From = new MailAddress("nabta2027@gmail.com"),
                Subject = subject,
                Body = msg,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);
            mailMessage.ReplyToList.Add(new MailAddress(from));

            await client.SendMailAsync(mailMessage);
        }
    }
}