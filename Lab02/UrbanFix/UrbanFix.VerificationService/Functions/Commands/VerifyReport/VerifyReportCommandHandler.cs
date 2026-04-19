using MassTransit;
using MediatR;
using UrbanFix.Common.Infrastructure;
using UrbanFix.VerificationService.Repository;

namespace UrbanFix.VerificationService.Functions.Commands.VerifyReport
{
    public class VerifyReportCommandHandler : IRequestHandler<VerifyReportCommand, bool>
    {
        private readonly IVerificationRepository _repository;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<VerifyReportCommandHandler> _logger;

        public VerifyReportCommandHandler(
            IVerificationRepository repository,
            IPublishEndpoint publish,
            ILogger<VerifyReportCommandHandler> logger)
        {
            _repository = repository;
            _publish = publish;
            _logger = logger;
        }

        public async Task<bool> Handle(VerifyReportCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Started verification for report {request.ReportId} with decision: {request.Decision}");

            var verification = await _repository.GetByReportIdAsync(request.ReportId);
            if (verification == null)
            {
                _logger.LogError($"[{GetType().Name}] Verification not found for report {request.ReportId}");
                return false;
            }

            verification.Decision = request.Decision;
            verification.Comment = request.Comment;
            verification.VerifiedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(verification);
            _logger.LogInformation($"[{GetType().Name}] Updated verification for report {request.ReportId}");

            await _publish.Publish(new ReportVerifiedEvent
            {
                ReportId = request.ReportId,
                SubmitterEmail = verification.SubmitterEmail,
                Decision = request.Decision,
                VerifiedAt = verification.VerifiedAt
            });
            _logger.LogInformation($"[{GetType().Name}] Published ReportVerifiedEvent for report {request.ReportId}");

            return true;
        }
    }
}
