namespace UrbanFix.NotificationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailProvider _provider;

        public EmailService(IEmailProvider provider)
        {
            _provider = provider;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            return await _provider.SendEmailAsync(toEmail, subject, body);
        }
    }
}
