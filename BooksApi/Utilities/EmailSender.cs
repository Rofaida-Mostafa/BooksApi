using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace  BooksApi.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("rofaidaashraf3@gmail.com", "cgph jimf qmgu oufp"),
                EnableSsl = true,
            };


            return client.SendMailAsync(
                new MailMessage(from: "rofaidaashraf3@gmail.com",
                                to: email,
                                subject,
                                htmlMessage
                                )
                {
                      IsBodyHtml= true
                }
             );

        }
    }
}
