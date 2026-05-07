using MailKit.Net.Smtp;
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
                var smtpHost = _configuration["Email:SmtpHost"] ?? "localhost";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "1025");
                var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@urbanfix.pl";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("UrbanFix", fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpHost, smtpPort, false);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"[{GetType().Name}] Email sent successfully via SMTP to {toEmail} - Subject: {subject}");
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
