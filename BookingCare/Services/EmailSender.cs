using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace BookingCare.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("ngminhquangg0301@gmail.com", "lynf iota uoza hzzm"),
                EnableSsl = true
            };
            var mailMessage = new MailMessage("ngminhquangg0301@gmail.com", email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };
            await client.SendMailAsync(mailMessage);
        }
    }
}
