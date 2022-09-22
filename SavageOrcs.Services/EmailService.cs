using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services
{
    public class EmailService : UnitOfWorkService, IEmailService
    {
        public EmailService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var from = new MailAddress("skripnik.petro@gmail.com", "Savage Orcs confirm email");
            var to = new MailAddress(email);

            var mailMessage = new MailMessage(from, to)
            {
                Subject = subject,
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true
            };

            mailMessage.Body += "<html>";
            mailMessage.Body += "<head>";
            mailMessage.Body += "<meta charset=\"utf-8\">";
            mailMessage.Body += "</head>";
            mailMessage.Body += "<body>";
            mailMessage.Body = message;
            mailMessage.Body += "</body>";
            mailMessage.Body += "</html>";

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("skripnik.petro@gmail.com", "okhkjtjepwpchnwu"),
                EnableSsl = true
            };
            await smtp.SendMailAsync(mailMessage);
        }
    }
}
