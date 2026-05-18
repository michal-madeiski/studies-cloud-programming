using MassTransit;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;
using UrbanFix.ReportService.Repository;

namespace UrbanFix.ReportService.Consumers
{
    public class ReportCreatedEventConsumer : IConsumer<ReportCreatedEvent>
    {
        private readonly IReportRepository _repository;
        private readonly ILogger<ReportCreatedEventConsumer> _logger;

        public ReportCreatedEventConsumer(IReportRepository repository, ILogger<ReportCreatedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReportCreatedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received ReportCreatedEvent for report {context.Message.ReportId}");

            var report = await _repository.GetByIdAsync(context.Message.ReportId);
            if (report == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Report {context.Message.ReportId} not found");
                return;
            }

            report.Status = ReportStatus.New;
            await _repository.UpdateAsync(report);

            _logger.LogInformation($"[{GetType().Name}] Set status New for report {context.Message.ReportId}");
        }
    }

    public class ReportVerifiedEventConsumer : IConsumer<ReportVerifiedEvent>
    {
        private readonly IReportRepository _repository;
        private readonly ILogger<ReportVerifiedEventConsumer> _logger;

        public ReportVerifiedEventConsumer(IReportRepository repository, ILogger<ReportVerifiedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReportVerifiedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received ReportVerifiedEvent for report {context.Message.ReportId}");

            var report = await _repository.GetByIdAsync(context.Message.ReportId);
            if (report == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Report {context.Message.ReportId} not found");
                return;
            }

            report.Status = context.Message.Decision == VerificationDecision.Accepted
                ? ReportStatus.Verified
                : ReportStatus.Rejected;

            await _repository.UpdateAsync(report);

            _logger.LogInformation($"[{GetType().Name}] Set status {report.Status} for report {context.Message.ReportId}");
        }
    }

    public class TaskAssignedEventConsumer : IConsumer<TaskAssignedEvent>
    {
        private readonly IReportRepository _repository;
        private readonly ILogger<TaskAssignedEventConsumer> _logger;

        public TaskAssignedEventConsumer(IReportRepository repository, ILogger<TaskAssignedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TaskAssignedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received TaskAssignedEvent for report {context.Message.ReportId}");

            var report = await _repository.GetByIdAsync(context.Message.ReportId);
            if (report == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Report {context.Message.ReportId} not found");
                return;
            }

            report.Status = ReportStatus.Assigned;
            await _repository.UpdateAsync(report);

            _logger.LogInformation($"[{GetType().Name}] Set status Assigned for report {context.Message.ReportId}");
        }
    }

    public class TaskCompletedEventConsumer : IConsumer<TaskCompletedEvent>
    {
        private readonly IReportRepository _repository;
        private readonly ILogger<TaskCompletedEventConsumer> _logger;

        public TaskCompletedEventConsumer(IReportRepository repository, ILogger<TaskCompletedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TaskCompletedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received TaskCompletedEvent for report {context.Message.ReportId}");

            var report = await _repository.GetByIdAsync(context.Message.ReportId);
            if (report == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Report {context.Message.ReportId} not found");
                return;
            }

            report.Status = ReportStatus.Completed;
            await _repository.UpdateAsync(report);

            _logger.LogInformation($"[{GetType().Name}] Set status Completed for report {context.Message.ReportId}");
        }
    }
}
