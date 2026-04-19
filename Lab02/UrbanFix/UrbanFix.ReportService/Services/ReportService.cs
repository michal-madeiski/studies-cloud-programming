using MassTransit;
using UrbanFix.Common.Infrastructure;
using UrbanFix.ReportService.Models;
using UrbanFix.ReportService.Repository;

namespace UrbanFix.ReportService.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;
        private readonly IS3FileStorageService _s3Storage;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IReportRepository repository,
            IS3FileStorageService s3Storage,
            IPublishEndpoint publish,
            ILogger<ReportService> logger)
        {
            _repository = repository;
            _s3Storage = s3Storage;
            _publish = publish;
            _logger = logger;
        }

        public async Task<Guid> CreateReportAsync(string email, string description, IFormFile file)
        {
            _logger.LogInformation($"[{GetType().Name}] Started creating report...");

            string fileName = file.FileName;
            string fileExtension = Path.GetExtension(fileName);
            string contentType = file.ContentType ?? "application/octet-stream";

            _logger.LogInformation($"[{GetType().Name}] File received: {fileName}, Extension: {fileExtension}, Size: {file.Length} bytes");

            try
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var (bucketName, objectKey, fileSize) = await _s3Storage.UploadFileAsync(
                        fileStream,
                        fileName,
                        contentType);

                    _logger.LogInformation($"[{GetType().Name}] File uploaded to S3 successfully");

                    var report = new Report
                    {
                        SubmitterEmail = email,
                        FileName = fileName,
                        FileExtension = fileExtension,
                        Description = description,
                        S3BucketName = bucketName,
                        S3ObjectKey = objectKey,
                        FileSize = fileSize,
                        UploadedAt = DateTime.UtcNow
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
            catch (Exception ex)
            {
                _logger.LogError($"[{GetType().Name}] Error creating report: {ex.Message}");
                throw;
            }
        }
    }
}
