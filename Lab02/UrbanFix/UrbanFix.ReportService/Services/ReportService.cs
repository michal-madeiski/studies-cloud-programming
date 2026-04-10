using MassTransit;
using UrbanFix.Common.Infrastructure;
using UrbanFix.ReportService.Models;
using UrbanFix.ReportService.Repository;

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
            _logger.LogInformation($"[{GetType().Name}] Started creating report...");

            string fileName = file.FileName;
            string fileExtension = Path.GetExtension(fileName);

            _logger.LogInformation($"[{GetType().Name}] File uploaded: {fileName}, Extension: {fileExtension}");

            var report = new Report
            {
                SubmitterEmail = email,
                FileName = fileName,
                FileExtension = fileExtension,
                Description = description
            };

            await _repository.AddAsync(report);

            var reportId = report.Id;
            _logger.LogInformation($"[{GetType().Name}] Added new report - {reportId}");

            await _publish.Publish(new ReportCreatedEvent
            {
                ReportId = reportId,
                SubmitterEmail = report.SubmitterEmail,
                Description = report.Description,
                CreatedAt = report.CreatedAt
            });
            _logger.LogInformation($"[{GetType().Name}] Publish event for new report - {reportId}");

            return reportId;
        }
    }
}
