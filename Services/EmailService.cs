using Azure.Messaging;
using Inmosat.ListenerDatFilesIridium.WorkerService.Configuration;
using Inmosat.ListenerDatFilesIridium.WorkerService.Models.EmailService;
using Inmosat.ListenerDatFilesIridium.WorkerService.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Inmosat.ListenerDatFilesIridium.WorkerService.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpSettings,
                            ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsyn(EmailData emailData)
        {
            var email     = new MimeMessage();
            email.Sender  = MailboxAddress.Parse(_smtpSettings.Username);
            email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.Username));
            email.To.Add(new MailboxAddress(emailData.recipientEmail, emailData.recipientEmail));
            email.Subject = email.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = GetPersonalizedHtml(emailData.recipientName, emailData.htmlMessage)
            };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);

                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrio un error en el metodo {nameof(SendEmailAsyn)}, de la clase {nameof(EmailService)}, detalle del errro: {ex.Message}");
                throw;
            }
            finally 
            {
                await smtp.DisconnectAsync(true);
            }
        }

        private string GetPersonalizedHtml(string name, string MessageContent)
        {
            return $@"
                <html>
                <body>
                    <h2>Hola, {name}!</h2>
                    <div style='border: 1px solid #ccc; padding: 15px; background-color: #f9f9f9;'>
                        <p>{MessageContent}</p>
                    </div>
                    <p>Saludos,</p>
                    <p>{_smtpSettings.SenderName}</p>
                </body>
                </html>";
        }
    }
}
