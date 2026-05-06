using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.ReportService.Functions.Commands.CreateReport;
using UrbanFix.ReportService.Functions.Queries.DownloadReportFile;
using UrbanFix.ReportService.Functions.Queries.GetAllReports;
using UrbanFix.ReportService.Functions.Queries.GetReport;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Controllers
{
    /// <summary>
    /// Reports management service
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create new report
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromForm] ReportRequest request)
        {
            var email = request.Email;
            var description = request.Description;
            var file = request.File;
            var latitude = request.Latitude;
            var longitude = request.Longitude;

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is required" });

            if (email == null || email == string.Empty)
                return BadRequest(new { message = "Email is required" });

            if (latitude == 0 || longitude == 0)
                return BadRequest(new { message = "Geographic coordinates are required" });

            var command = new CreateReportCommand(email, description, file, latitude, longitude);
            var reportId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetReport), new { reportId }, new { reportId });
        }

        /// <summary>
        /// Get report details
        /// </summary>
        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetReport(Guid reportId)
        {
            var query = new GetReportQuery(reportId);
            var report = await _mediator.Send(query);

            if (report == null)
                return NotFound(new { message = "Report not found" });

            return Ok(new
            {
                report.Id,
                report.SubmitterEmail,
                report.FileName,
                report.FileExtension,
                report.Description,
                report.Latitude,
                report.Longitude,
                report.FileSize,
                report.UploadedAt,
                report.S3ObjectKey
            });
        }

        /// <summary>
        /// Download report photo
        /// </summary>
        [HttpGet("{reportId}/photo")]
        public async Task<IActionResult> DownloadReportPhoto(Guid reportId)
        {
            var downloadQuery = new DownloadReportFileQuery(reportId);
            var result = await _mediator.Send(downloadQuery);

            if (result == null)
                return NotFound(new { message = "Report not found" });

            return File(result.FileStream, "application/octet-stream", result.Report.FileName);
        }

        /// <summary>
        /// Get all reports
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var query = new GetAllReportsQuery();
            var reports = await _mediator.Send(query);

            return Ok(reports.Select(r => new
            {
                r.Id,
                r.SubmitterEmail,
                r.FileName,
                r.FileExtension,
                r.Description,
                r.Latitude,
                r.Longitude,
                r.FileSize,
                r.UploadedAt,
                r.S3ObjectKey
            }));
        }
    }
}
