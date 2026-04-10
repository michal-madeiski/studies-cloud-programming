using MassTransit;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;
using UrbanFix.NotificationService.Models;
using UrbanFix.NotificationService.Repository;

namespace UrbanFix.NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository repository, IPublishEndpoint publish, ILogger<NotificationService> logger)
        {
            _repository = repository;
            _publish = publish;
            _logger = logger;
        }

        public async Task<bool> SendNotificationAsync(Guid reportId, string? recipientEmail, TaskAssignmentStatus status, string description)
        {
            _logger.LogInformation($"[{GetType().Name}] Started sending notification for report {reportId} to {recipientEmail}");

            var statusText = status.ToString();
            var messageBody = $"Report {reportId}: {description} - Status: {statusText}";

            var notification = new Notification
            {
                ReportId = reportId,
                RecipientEmail = recipientEmail,
                MessageBody = messageBody
            };

            await _repository.AddAsync(notification);
            _logger.LogInformation($"[{GetType().Name}] Saved notification to database for report {reportId}");

            _logger.LogInformation($"[{GetType().Name}] [EMAIL LOG] Sending email to {recipientEmail} - Subject: Report Status Update - Body: {messageBody}");

            await _publish.Publish(new NotificationSentEvent
            {
                ReportId = reportId,
                RecipientEmail = recipientEmail,
                Status = status,
                SentAt = DateTime.UtcNow
            });
            _logger.LogInformation($"[{GetType().Name}] Published NotificationSentEvent for report {reportId}");

            return true;
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByReportIdAsync(Guid reportId)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving notifications for report {reportId}");
            return await _repository.GetByReportIdAsync(reportId);
        }
    }
}
