using MassTransit;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;
using UrbanFix.VerificationService.Models;
using UrbanFix.VerificationService.Repository;

namespace UrbanFix.VerificationService.Consumers
{
    public class ReportCreatedEventConsumer : IConsumer<ReportCreatedEvent>
    {
        private readonly IVerificationRepository _repository;
        private readonly ILogger<ReportCreatedEventConsumer> _logger;

        public ReportCreatedEventConsumer(IVerificationRepository repository, ILogger<ReportCreatedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReportCreatedEvent> context)
        {
            _logger.LogInformation($"[{GetType().Name}] Received ReportCreatedEvent for report {context.Message.ReportId}");

            var verification = new Verification
            {
                ReportId = context.Message.ReportId,
                SubmitterEmail = context.Message.SubmitterEmail,
                Decision = VerificationDecision.Pending
            };

            await _repository.AddAsync(verification);
            _logger.LogInformation($"[{GetType().Name}] Created verification record for report {context.Message.ReportId}");
        }
    }
}
