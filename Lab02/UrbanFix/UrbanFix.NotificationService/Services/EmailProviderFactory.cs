namespace UrbanFix.NotificationService.Services
{
    public interface IEmailProviderFactory
    {
        IEmailProvider CreateProvider();
    }

    public class EmailProviderFactory : IEmailProviderFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailProviderFactory> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EmailProviderFactory(
            IConfiguration configuration,
            ILogger<EmailProviderFactory> logger,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IEmailProvider CreateProvider()
        {
            var provider = _configuration["Email:Provider"] ?? "Smtp";

            if (provider.Equals("AwsSes", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("[EmailProviderFactory] Using AWS SES email provider");
                return _serviceProvider.GetRequiredService<AwsSesEmailProvider>();
            }

            _logger.LogInformation("[EmailProviderFactory] Using SMTP email provider");
            return _serviceProvider.GetRequiredService<SmtpEmailProvider>();
        }
    }
}
