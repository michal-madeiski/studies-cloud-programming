using MediatR;
using UrbanFix.ReportService.Models;
using UrbanFix.ReportService.Repository;

namespace UrbanFix.ReportService.Functions.Queries.GetAllReports
{
    public class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, IEnumerable<Report>>
    {
        private readonly IReportRepository _repository;
        private readonly ILogger<GetAllReportsQueryHandler> _logger;

        public GetAllReportsQueryHandler(IReportRepository repository, ILogger<GetAllReportsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Report>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving all reports");

            var reports = await _repository.GetAllAsync();

            _logger.LogInformation($"[{GetType().Name}] Retrieved {reports.Count()} reports");

            return reports;
        }
    }
}
