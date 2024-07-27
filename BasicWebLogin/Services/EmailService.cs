using BasicWebLogin.Models;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net;
using System.Net.Mail;

namespace BasicWebLogin.Services
{
    public static class EmailService
    {
        private static IConfiguration GetAppConfiguration()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json", false, false).Build();
        }

        private static EmailConfiguration GetEmailConfiguration()
        {
            return new EmailConfiguration()
            {
                Email = GetAppConfiguration().GetValue<string>("Credentials:Email"),
                SMTPServer = GetAppConfiguration().GetValue<string>("Credentials:Server"),
                Password = GetAppConfiguration().GetValue<string>("Credentials:Password"),
                Port = GetAppConfiguration().GetValue<int>("Credentials:Port")
            };
        }

        public static Task SendEmail(EmailModel email)
        {
            try
            {
                EmailConfiguration emailConfig = GetEmailConfiguration();
                SmtpClient smtpClient = new SmtpClient(emailConfig.SMTPServer, emailConfig.Port)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(emailConfig.Email, emailConfig.Password)
                };

                return smtpClient.SendMailAsync(
                    new MailMessage(from: emailConfig.Email, to: email.To, email.Subject, email.Content)
                    {
                        IsBodyHtml = true
                    });
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
    }
}
