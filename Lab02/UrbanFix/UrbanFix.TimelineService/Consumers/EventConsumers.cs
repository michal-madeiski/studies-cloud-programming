using MassTransit;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;
using UrbanFix.TimelineService.Models;
using UrbanFix.TimelineService.Repository;

namespace UrbanFix.TimelineService.Consumers
{
    public class ReportCreatedEventConsumer : IConsumer<ReportCreatedEvent>
    {
        private readonly ITimelineRepository _repository;
        private readonly ILogger<ReportCreatedEventConsumer> _logger;

        public ReportCreatedEventConsumer(ITimelineRepository repository, ILogger<ReportCreatedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReportCreatedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received ReportCreatedEvent for report {context.Message.ReportId}");

            var timeline = new Timeline
            {
                ReportId = context.Message.ReportId,
                NewStatus = TaskAssignmentStatus.New,
                Description = "Report created by resident",
                OccurredAt = context.Message.CreatedAt
            };

            await _repository.AddAsync(timeline);
            _logger.LogInformation($"[{GetType().Name}] Created timeline entry for report {context.Message.ReportId}");
        }
    }

    public class ReportVerifiedEventConsumer : IConsumer<ReportVerifiedEvent>
    {
        private readonly ITimelineRepository _repository;
        private readonly ILogger<ReportVerifiedEventConsumer> _logger;

        public ReportVerifiedEventConsumer(ITimelineRepository repository, ILogger<ReportVerifiedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReportVerifiedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received ReportVerifiedEvent for report {context.Message.ReportId}");

            var status = context.Message.Decision == VerificationDecision.Accepted ? TaskAssignmentStatus.InProgress : TaskAssignmentStatus.Rejected;
            var decisionText = context.Message.Decision == VerificationDecision.Accepted ? "accepted" : "rejected";

            var timeline = new Timeline
            {
                ReportId = context.Message.ReportId,
                NewStatus = status,
                Description = $"Report {decisionText} during verification",
                OccurredAt = context.Message.VerifiedAt
            };

            await _repository.AddAsync(timeline);
            _logger.LogInformation($"[{GetType().Name}] Created timeline entry for verified report {context.Message.ReportId}");
        }
    }

    public class TaskAssignedEventConsumer : IConsumer<TaskAssignedEvent>
    {
        private readonly ITimelineRepository _repository;
        private readonly ILogger<TaskAssignedEventConsumer> _logger;

        public TaskAssignedEventConsumer(ITimelineRepository repository, ILogger<TaskAssignedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TaskAssignedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received TaskAssignedEvent for report {context.Message.ReportId}");

            var timeline = new Timeline
            {
                ReportId = context.Message.ReportId,
                NewStatus = TaskAssignmentStatus.InProgress,
                Description = $"Task assigned to technical team {context.Message.AssignedTeamId}",
                OccurredAt = context.Message.AssignedAt
            };

            await _repository.AddAsync(timeline);
            _logger.LogInformation($"[{GetType().Name}] Created timeline entry for assigned task {context.Message.ReportId}");
        }
    }

    // NotificationSentEventConsumer removed - no need to create timeline entry for status changes
    // Timeline entries are created only from TaskAssignedEvent, ReportVerifiedEvent, ReportCreatedEvent, and TaskCompletedEvent

    public class TaskCompletedEventConsumer : IConsumer<TaskCompletedEvent>
    {
        private readonly ITimelineRepository _repository;
        private readonly ILogger<TaskCompletedEventConsumer> _logger;

        public TaskCompletedEventConsumer(ITimelineRepository repository, ILogger<TaskCompletedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TaskCompletedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received TaskCompletedEvent for report {context.Message.ReportId}");

            var timeline = new Timeline
            {
                ReportId = context.Message.ReportId,
                NewStatus = TaskAssignmentStatus.Completed,
                Description = "Technical team completed the work",
                OccurredAt = context.Message.CompletedAt
            };

            await _repository.AddAsync(timeline);
            _logger.LogInformation($"[{GetType().Name}] Created timeline entry for completed task {context.Message.ReportId}");
        }
    }
}
