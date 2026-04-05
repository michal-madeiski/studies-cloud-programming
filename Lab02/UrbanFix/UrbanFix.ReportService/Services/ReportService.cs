using MassTransit;
using UrbanFix.ReportService.Repository;
using UrbanFix.ReportService.Models;
using UrbanFix.ReportService.Infrastracture;

namespace UrbanFix.ReportService.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IReportRepository repository, IPublishEndpoint publish, ILogger<ReportService> logger)
        {
            _repository = repository;
            _publish = publish;
            _logger = logger;
        }

        public async Task<Guid> CreateReportAsync(string email, string description, IFormFile file)
        {
            _logger.LogInformation("Started creating report...");

            string fileName = file.FileName;
            string fileExtension = Path.GetExtension(fileName);

            var report = new Report
            {
                SubmitterEmail = email,
                FileName = fileName,
                FileExtension = fileExtension,
                Description = description
            };

            await _repository.AddAsync(report);

            var reportId = report.Id;
            _logger.LogInformation($"Added new report - {reportId}");

            await _publish.Publish(new ReportCreatedEvent
            {
                ReportId = reportId,
                SubmitterEmail = report.SubmitterEmail,
                Description = report.Description,
                CreatedAt = report.CreatedAt
            });
            _logger.LogInformation($"Publish event for new report - {reportId}");

            return reportId;
        }
    }
}
