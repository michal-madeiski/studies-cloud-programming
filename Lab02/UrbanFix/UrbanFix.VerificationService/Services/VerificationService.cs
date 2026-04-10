using MassTransit;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;
using UrbanFix.VerificationService.Repository;

namespace UrbanFix.VerificationService.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IVerificationRepository _repository;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<VerificationService> _logger;

        public VerificationService(IVerificationRepository repository, IPublishEndpoint publish, ILogger<VerificationService> logger)
        {
            _repository = repository;
            _publish = publish;
            _logger = logger;
        }

        public async Task<Guid?> GetVerificationByReportIdAsync(Guid reportId)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving verification for report {reportId}");
            var verification = await _repository.GetByReportIdAsync(reportId);
            return verification?.Id;
        }

        public async Task<bool> VerifyReportAsync(Guid reportId, VerificationDecision decision, string? comment)
        {
            _logger.LogInformation($"[{GetType().Name}] Started verification for report {reportId} with decision: {decision}");

            var verification = await _repository.GetByReportIdAsync(reportId);
            if (verification == null)
            {
                _logger.LogError($"[{GetType().Name}] Verification not found for report {reportId}");
                return false;
            }

            verification.Decision = decision;
            verification.Comment = comment;
            verification.VerifiedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(verification);
            _logger.LogInformation($"[{GetType().Name}] Updated verification for report {reportId}");

            await _publish.Publish(new ReportVerifiedEvent
            {
                ReportId = reportId,
                SubmitterEmail = verification.SubmitterEmail,
                Decision = decision,
                VerifiedAt = verification.VerifiedAt
            });
            _logger.LogInformation($"[{GetType().Name}] Published ReportVerifiedEvent for report {reportId}");

            return true;
        }
    }
}
