using MassTransit;
using MediatR;
using UrbanFix.Common.Infrastructure;
using UrbanFix.ReportService.Models;
using UrbanFix.ReportService.Repository;
using UrbanFix.ReportService.Services;

namespace UrbanFix.ReportService.Functions.Commands.CreateReport
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Guid>
    {
        private readonly IReportRepository _repository;
        private readonly IS3FileStorageService _s3Storage;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<CreateReportCommandHandler> _logger;

        public CreateReportCommandHandler(
            IReportRepository repository,
            IS3FileStorageService s3Storage,
            IPublishEndpoint publish,
            ILogger<CreateReportCommandHandler> logger)
        {
            _repository = repository;
            _s3Storage = s3Storage;
            _publish = publish;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Started creating report...");

            string fileName = request.File.FileName;
            string fileExtension = Path.GetExtension(fileName);
            string contentType = request.File.ContentType ?? "application/octet-stream";

            _logger.LogInformation($"[{GetType().Name}] File received: {fileName}, Extension: {fileExtension}, Size: {request.File.Length} bytes");

            try
            {
                using (var fileStream = request.File.OpenReadStream())
                {
                    var (bucketName, objectKey, fileSize) = await _s3Storage.UploadFileAsync(
                        fileStream,
                        fileName,
                        contentType);

                    _logger.LogInformation($"[{GetType().Name}] File uploaded to S3 successfully");

                    var report = new Report
                    {
                        SubmitterEmail = request.Email,
                        FileName = fileName,
                        FileExtension = fileExtension,
                        Description = request.Description,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
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
                        CreatedAt = report.UploadedAt
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
