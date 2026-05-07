namespace UrbanFix.NotificationService.Services
{
    public interface IEmailProvider
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }
}
