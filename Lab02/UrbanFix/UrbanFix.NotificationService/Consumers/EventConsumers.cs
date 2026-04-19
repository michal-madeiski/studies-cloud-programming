using MassTransit;
using MediatR;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;
using UrbanFix.NotificationService.Functions.Commands.SendNotification;

namespace UrbanFix.NotificationService.Consumers
{
    public class TaskAssignedEventConsumer : IConsumer<TaskAssignedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TaskAssignedEventConsumer> _logger;

        public TaskAssignedEventConsumer(IMediator mediator, ILogger<TaskAssignedEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TaskAssignedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received TaskAssignedEvent for report {context.Message.ReportId}");

            await _mediator.Send(new SendNotificationCommand(
                context.Message.ReportId,
                context.Message.SubmitterEmail,
                TaskAssignmentStatus.InProgress,
                "Your report has been assigned to technical team for further processing"
            ));

            _logger.LogInformation($"[{GetType().Name}] Sent notification for assigned task for report {context.Message.ReportId}");
        }
    }

    public class ReportVerifiedEventConsumer : IConsumer<ReportVerifiedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportVerifiedEventConsumer> _logger;

        public ReportVerifiedEventConsumer(IMediator mediator, ILogger<ReportVerifiedEventConsumer> logger)
        {
            _mediator = mediator;
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

            await _mediator.Send(new SendNotificationCommand(
                context.Message.ReportId,
                context.Message.SubmitterEmail,
                status,
                message
            ));

            _logger.LogInformation($"[{GetType().Name}] Sent notification for verified report for report {context.Message.ReportId}");
        }
    }

    public class TaskCompletedEventConsumer : IConsumer<TaskCompletedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TaskCompletedEventConsumer> _logger;

        public TaskCompletedEventConsumer(IMediator mediator, ILogger<TaskCompletedEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TaskCompletedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received TaskCompletedEvent for report {context.Message.ReportId}");

            await _mediator.Send(new SendNotificationCommand(
                context.Message.ReportId,
                context.Message.SubmitterEmail,
                TaskAssignmentStatus.Completed,
                "Technical team has completed the work on your report"
            ));

            _logger.LogInformation($"[{GetType().Name}] Sent notification for completed task for report {context.Message.ReportId}");
        }
    }
}
