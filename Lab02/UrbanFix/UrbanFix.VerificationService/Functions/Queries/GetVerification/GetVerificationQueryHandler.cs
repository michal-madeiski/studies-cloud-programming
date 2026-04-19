using MediatR;
using UrbanFix.VerificationService.Repository;

namespace UrbanFix.VerificationService.Functions.Queries.GetVerification
{
    public class GetVerificationQueryHandler : IRequestHandler<GetVerificationQuery, Guid?>
    {
        private readonly IVerificationRepository _repository;
        private readonly ILogger<GetVerificationQueryHandler> _logger;

        public GetVerificationQueryHandler(IVerificationRepository repository, ILogger<GetVerificationQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Guid?> Handle(GetVerificationQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving verification for report {request.ReportId}");
            var verification = await _repository.GetByReportIdAsync(request.ReportId);
            return verification?.Id;
        }
    }
}
