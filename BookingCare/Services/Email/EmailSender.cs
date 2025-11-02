using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace BookingCare.Services.Email
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("nguyenminhquangg03012004@gmail.com", "yqcz ibyy zsxm zuzq"),
                EnableSsl = true
            };
            var mailMessage = new MailMessage("nguyenminhquangg03012004@gmail.com", email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };
            await client.SendMailAsync(mailMessage);
        }
    }
}
