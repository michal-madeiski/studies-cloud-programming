namespace UrbanFix.NotificationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailProvider _provider;

        public EmailService(IEmailProviderFactory factory)
        {
            _provider = factory.CreateProvider();
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            return await _provider.SendEmailAsync(toEmail, subject, body);
        }
    }
}
