using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace UrbanFix.NotificationService.Services
{
    public class AwsSesEmailProvider : IEmailProvider
    {
        private readonly IAmazonSimpleEmailService _sesClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AwsSesEmailProvider> _logger;

        public AwsSesEmailProvider(IConfiguration configuration, ILogger<AwsSesEmailProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var region = _configuration["AWS:Region"] ?? "eu-central-1";
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var sessionToken = _configuration["AWS:SessionToken"];

            AWSCredentials credentials;
            if (!string.IsNullOrEmpty(sessionToken))
            {
                credentials = new SessionAWSCredentials(accessKey, secretKey, sessionToken);
            }
            else
            {
                credentials = new BasicAWSCredentials(accessKey, secretKey);
            }

            _sesClient = new AmazonSimpleEmailServiceClient(credentials, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@urbanfix.pl";

                var sendRequest = new SendEmailRequest
                {
                    Source = fromEmail,
                    Destination = new Destination { ToAddresses = new List<string> { toEmail } },
                    Message = new Message
                    {
                        Subject = new Content { Data = subject, Charset = "UTF-8" },
                        Body = new Body
                        {
                            Html = new Content { Data = body, Charset = "UTF-8" }
                        }
                    }
                };

                var response = await _sesClient.SendEmailAsync(sendRequest);

                _logger.LogInformation($"[{GetType().Name}] Email sent successfully via AWS SES to {toEmail} - MessageId: {response.MessageId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{GetType().Name}] Failed to send email via AWS SES to {toEmail}");
                return false;
            }
        }
    }
}
