using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BookingCare.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.From, _settings.Password),
                EnableSsl = true
            };
            var mailMessage = new MailMessage(_settings.From, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };
            await client.SendMailAsync(mailMessage);
        }
    }
}
