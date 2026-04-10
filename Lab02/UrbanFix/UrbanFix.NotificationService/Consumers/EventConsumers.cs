using MassTransit;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;
using UrbanFix.NotificationService.Services;

namespace UrbanFix.NotificationService.Consumers
{
    public class TaskAssignedEventConsumer : IConsumer<TaskAssignedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<TaskAssignedEventConsumer> _logger;

        public TaskAssignedEventConsumer(INotificationService notificationService, ILogger<TaskAssignedEventConsumer> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TaskAssignedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received TaskAssignedEvent for report {context.Message.ReportId}");

            await _notificationService.SendNotificationAsync(
                context.Message.ReportId,
                context.Message.SubmitterEmail,
                TaskAssignmentStatus.InProgress,
                $"Your report has been assigned to technical team for further processing"
            );

            _logger.LogInformation($"[{GetType().Name}] Sent notification for assigned task for report {context.Message.ReportId}");
        }
    }

    public class ReportVerifiedEventConsumer : IConsumer<ReportVerifiedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReportVerifiedEventConsumer> _logger;

        public ReportVerifiedEventConsumer(INotificationService notificationService, ILogger<ReportVerifiedEventConsumer> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReportVerifiedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received ReportVerifiedEvent for report {context.Message.ReportId}");

            var status = context.Message.Decision == VerificationDecision.Accepted
                ? TaskAssignmentStatus.InProgress
                : TaskAssignmentStatus.Rejected;

            var message = context.Message.Decision == VerificationDecision.Accepted
                ? "Your report has been verified and accepted for processing"
                : "Your report has been reviewed and rejected";

            await _notificationService.SendNotificationAsync(
                context.Message.ReportId,
                context.Message.SubmitterEmail,
                status,
                message
            );

            _logger.LogInformation($"[{GetType().Name}] Sent notification for verified report for report {context.Message.ReportId}");
        }
    }

    public class TaskCompletedEventConsumer : IConsumer<TaskCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<TaskCompletedEventConsumer> _logger;

        public TaskCompletedEventConsumer(INotificationService notificationService, ILogger<TaskCompletedEventConsumer> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TaskCompletedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received TaskCompletedEvent for report {context.Message.ReportId}");

            await _notificationService.SendNotificationAsync(
                context.Message.ReportId,
                context.Message.SubmitterEmail,
                TaskAssignmentStatus.Completed,
                "Technical team has completed the work on your report"
            );

            _logger.LogInformation($"[{GetType().Name}] Sent notification for completed task for report {context.Message.ReportId}");
        }
    }
}
