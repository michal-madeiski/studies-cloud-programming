using MediatR;
using UrbanFix.ReportService.Models;
using UrbanFix.ReportService.Repository;

namespace UrbanFix.ReportService.Functions.Queries.GetReport
{
    public class GetReportQueryHandler : IRequestHandler<GetReportQuery, Report?>
    {
        private readonly IReportRepository _repository;
        private readonly ILogger<GetReportQueryHandler> _logger;

        public GetReportQueryHandler(IReportRepository repository, ILogger<GetReportQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Report?> Handle(GetReportQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving report {request.ReportId}");

            var report = await _repository.GetByIdAsync(request.ReportId);

            if (report == null)
                _logger.LogWarning($"[{GetType().Name}] Report {request.ReportId} not found");
            else
                _logger.LogInformation($"[{GetType().Name}] Found report {request.ReportId}");

            return report;
        }
    }
}
