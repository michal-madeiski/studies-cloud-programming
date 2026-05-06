using MediatR;
using UrbanFix.ReportService.Repository;
using UrbanFix.ReportService.Services;

namespace UrbanFix.ReportService.Functions.Queries.DownloadReportFile
{
    public class DownloadReportFileQueryHandler : IRequestHandler<DownloadReportFileQuery, DownloadReportFileResult?>
    {
        private readonly IReportRepository _repository;
        private readonly IS3FileStorageService _s3Storage;
        private readonly ILogger<DownloadReportFileQueryHandler> _logger;

        public DownloadReportFileQueryHandler(
            IReportRepository repository,
            IS3FileStorageService s3Storage,
            ILogger<DownloadReportFileQueryHandler> logger)
        {
            _repository = repository;
            _s3Storage = s3Storage;
            _logger = logger;
        }

        public async Task<DownloadReportFileResult?> Handle(DownloadReportFileQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Starting download for report {request.ReportId}");

            var report = await _repository.GetByIdAsync(request.ReportId);
            if (report == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Report {request.ReportId} not found");
                return null;
            }

            try
            {
                var fileStream = await _s3Storage.DownloadFileAsync(report.S3BucketName, report.S3ObjectKey);
                _logger.LogInformation($"[{GetType().Name}] Successfully retrieved file stream for report {request.ReportId}");

                return new DownloadReportFileResult(report, fileStream);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{GetType().Name}] Error downloading file for report {request.ReportId}: {ex.Message}");
                throw;
            }
        }
    }
}
