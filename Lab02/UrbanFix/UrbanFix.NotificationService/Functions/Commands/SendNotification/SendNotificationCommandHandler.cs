using MassTransit;
using MediatR;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;
using UrbanFix.NotificationService.Models;
using UrbanFix.NotificationService.Repository;
using UrbanFix.NotificationService.Services;

namespace UrbanFix.NotificationService.Functions.Commands.SendNotification
{
    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, bool>
    {
        private readonly INotificationRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<SendNotificationCommandHandler> _logger;

        public SendNotificationCommandHandler(
            INotificationRepository repository,
            IEmailService emailService,
            IPublishEndpoint publish,
            ILogger<SendNotificationCommandHandler> logger)
        {
            _repository = repository;
            _emailService = emailService;
            _publish = publish;
            _logger = logger;
        }

        public async Task<bool> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Started sending notification for report {request.ReportId} to {request.RecipientEmail}");

            string subject = GetEmailSubject(request.Status);
            string messageBody = GetEmailBody(request);

            var notification = new Notification
            {
                ReportId = request.ReportId,
                RecipientEmail = request.RecipientEmail,
                MessageBody = messageBody
            };

            await _repository.AddAsync(notification);
            _logger.LogInformation($"[{GetType().Name}] Saved notification to database for report {request.ReportId}");

            var emailSent = await _emailService.SendEmailAsync(
                request.RecipientEmail,
                subject,
                messageBody);

            if (!emailSent)
            {
                _logger.LogWarning($"[{GetType().Name}] Failed to send email to {request.RecipientEmail}");
            }

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

        private string GetEmailSubject(TaskAssignmentStatus status)
        {
            return status switch
            {
                TaskAssignmentStatus.New => "Potwierdzenie otrzymania zgłoszenia",
                TaskAssignmentStatus.InProgress => "Aktualizacja statusu zgłoszenia",
                TaskAssignmentStatus.Completed => "Zgłoszenie ukończone",
                TaskAssignmentStatus.Rejected => "Decyzja dotycząca zgłoszenia",
                _ => "Powiadomienie dotycząca zgłoszenia"
            };
        }

        private string GetEmailBody(SendNotificationCommand request)
        {
            return request.Status switch
            {
                TaskAssignmentStatus.New => $"Otrzymaliśmy twoje zgłoszenie o numerze {request.ReportId}",

                TaskAssignmentStatus.InProgress => $"Twoje zgłoszenie #{request.ReportId}: {request.Description}",

                TaskAssignmentStatus.Rejected =>
                    $"Twoje zgłoszenie #{request.ReportId} zostało zweryfikowane i odrzucone.",

                TaskAssignmentStatus.Completed =>
                    $"Twoje zgłoszenie #{request.ReportId} zostało ukończone.",

                _ => $"Status zgłoszenia #{request.ReportId}: {request.Description}"
            };
        }
    }
}
