using MassTransit;
using MediatR;
using UrbanFix.AssignmentService.Functions.Commands.AssignTask;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;

namespace UrbanFix.AssignmentService.Consumers
{
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

            if (context.Message.Decision == VerificationDecision.Accepted)
            {
                _logger.LogInformation($"[{GetType().Name}] Report {context.Message.ReportId} was accepted, assigning task");
                await _mediator.Send(new AssignTaskCommand(context.Message.ReportId, context.Message.SubmitterEmail));
            }
            else
            {
                _logger.LogInformation($"[{GetType().Name}] Report {context.Message.ReportId} was {context.Message.Decision}, skipping assignment");
            }
        }
    }
}
