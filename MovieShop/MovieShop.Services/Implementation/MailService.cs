using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MovieShop.Domain;
using MovieShop.Domain.DomainModels;
using MovieShop.Services.Interface;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Services.Implementation
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var emailMessage = new MimeMessage
            {
                Sender = new MailboxAddress(_mailSettings.SendersName, _mailSettings.SmtpUserName),
                Subject = mailRequest.Subject
            };

            emailMessage.From.Add(new MailboxAddress(_mailSettings.EmailDisplayName, _mailSettings.SmtpUserName));

            emailMessage.Body = new TextPart(TextFormat.Plain) { Text = mailRequest.Body };

            emailMessage.To.Add(new MailboxAddress(mailRequest.ToEmail));

            try
            {
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    var socketOption = _mailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                    await smtp.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.SmtpServerPort, socketOption);

                    if (!string.IsNullOrEmpty(_mailSettings.SmtpUserName))
                    {
                        await smtp.AuthenticateAsync(_mailSettings.SmtpUserName, _mailSettings.SmtpPassword);
                    }

                     await smtp.SendAsync(emailMessage);
                  
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }
    }
}
