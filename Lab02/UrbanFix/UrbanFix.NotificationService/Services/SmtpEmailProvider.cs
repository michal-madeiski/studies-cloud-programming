using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace UrbanFix.NotificationService.Services
{
    public class SmtpEmailProvider : IEmailProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailProvider> _logger;

        public SmtpEmailProvider(IConfiguration configuration, ILogger<SmtpEmailProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var provider = _configuration["Email:Provider"];
                var fromEmail = _configuration["Email:FromEmail"];

                var smtpHost = _configuration[$"Email:Providers:{provider}:SmtpHost"];
                var smtpPort = int.Parse(_configuration[$"Email:Providers:{provider}:SmtpPort"]);
                var smtpUser = _configuration[$"Email:Providers:{provider}:SmtpUser"];
                var smtpPassword = _configuration[$"Email:Providers:{provider}:SmtpPassword"];
                var useStartTls = bool.Parse(_configuration[$"Email:Providers:{provider}:UseStartTls"]);
                var useAuthentication = !string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("UrbanFix", fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                    SecureSocketOptions secureSocketOptions = SecureSocketOptions.None;
                    if (useStartTls)
                    {
                        secureSocketOptions = SecureSocketOptions.StartTls;
                    }
                    else if (smtpPort == 465)
                    {
                        secureSocketOptions = SecureSocketOptions.SslOnConnect;
                    }

                    await client.ConnectAsync(smtpHost, smtpPort, secureSocketOptions);

                    if (useAuthentication)
                    {
                        await client.AuthenticateAsync(smtpUser, smtpPassword);
                    }

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"[{GetType().Name}] Email sent successfully via SMTP ({provider}) to {toEmail} - Subject: {subject}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{GetType().Name}] Failed to send email via SMTP to {toEmail}");
                return false;
            }
        }
    }
}
