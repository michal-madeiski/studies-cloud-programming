using MassTransit;
using UrbanFix.AssignmentService.Services;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;

namespace UrbanFix.AssignmentService.Consumers
{
    public class ReportVerifiedEventConsumer : IConsumer<ReportVerifiedEvent>
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ILogger<ReportVerifiedEventConsumer> _logger;

        public ReportVerifiedEventConsumer(IAssignmentService assignmentService, ILogger<ReportVerifiedEventConsumer> logger)
        {
            _assignmentService = assignmentService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReportVerifiedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received ReportVerifiedEvent for report {context.Message.ReportId}");

            if (context.Message.Decision == VerificationDecision.Accepted)
            {
                _logger.LogInformation($"[{GetType().Name}] Report {context.Message.ReportId} was accepted, assigning task");
                await _assignmentService.AssignTaskAsync(context.Message.ReportId, context.Message.SubmitterEmail);
            }
            else
            {
                _logger.LogInformation($"[{GetType().Name}] Report {context.Message.ReportId} was {context.Message.Decision}, skipping assignment");
            }
        }
    }
}
