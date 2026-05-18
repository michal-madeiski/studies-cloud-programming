using MediatR;
using UrbanFix.Common.Pagination;
using UrbanFix.ReportService.Models;
using UrbanFix.ReportService.Repository;

namespace UrbanFix.ReportService.Functions.Queries.GetAllReports
{
    public class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, PaginationResponse<Report>>
    {
        private readonly IReportRepository _repository;
        private readonly ILogger<GetAllReportsQueryHandler> _logger;

        public GetAllReportsQueryHandler(IReportRepository repository, ILogger<GetAllReportsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PaginationResponse<Report>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving reports with pagination - PageNumber: {request.PageNumber}, PageSize: {request.PageSize}, SortDescending: {request.SortDescending}, From: {request.From}, To: {request.To}");

            var reports = await _repository.GetAllAsync(request.PageNumber, request.PageSize, request.SortDescending, request.From, request.To, request.Status);

            _logger.LogInformation($"[{GetType().Name}] Retrieved {reports.Items.Count} reports from {reports.TotalCount} total");

            return reports;
        }
    }
}
