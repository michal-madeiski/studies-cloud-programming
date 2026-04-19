using MassTransit;
using MediatR;
using UrbanFix.Common.Infrastructure;
using UrbanFix.NotificationService.Models;
using UrbanFix.NotificationService.Repository;

namespace UrbanFix.NotificationService.Functions.Commands.SendNotification
{
    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, bool>
    {
        private readonly INotificationRepository _repository;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<SendNotificationCommandHandler> _logger;

        public SendNotificationCommandHandler(
            INotificationRepository repository,
            IPublishEndpoint publish,
            ILogger<SendNotificationCommandHandler> logger)
        {
            _repository = repository;
            _publish = publish;
            _logger = logger;
        }

        public async Task<bool> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Started sending notification for report {request.ReportId} to {request.RecipientEmail}");

            var statusText = request.Status.ToString();
            var messageBody = $"Report {request.ReportId}: {request.Description} - Status: {statusText}";

            var notification = new Notification
            {
                ReportId = request.ReportId,
                RecipientEmail = request.RecipientEmail,
                MessageBody = messageBody
            };

            await _repository.AddAsync(notification);
            _logger.LogInformation($"[{GetType().Name}] Saved notification to database for report {request.ReportId}");

            _logger.LogInformation($"[{GetType().Name}] [EMAIL LOG] Sending email to {request.RecipientEmail} - Subject: Report Status Update - Body: {messageBody}");

            await _publish.Publish(new NotificationSentEvent
            {
                ReportId = request.ReportId,
                RecipientEmail = request.RecipientEmail,
                Status = request.Status,
                SentAt = DateTime.UtcNow
            });
            _logger.LogInformation($"[{GetType().Name}] Published NotificationSentEvent for report {request.ReportId}");

            return true;
        }
    }
}
