using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.ReportService.Functions.Commands.CreateReport;
using UrbanFix.ReportService.Functions.Queries.GetAllReports;
using UrbanFix.ReportService.Functions.Queries.GetReport;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromForm] ReportRequest request)
        {
            var email = request.Email;
            var description = request.Description;
            var file = request.File;
            var latitude = request.Latitude;
            var longitude = request.Longitude;

            if (file == null || file.Length == 0)
                return BadRequest("File is necessary");

            if (email == null || email == string.Empty)
                return BadRequest("Email is necessary");

            if (latitude == 0 || longitude == 0)
                return BadRequest("Geographic coordinates are necessary");

            var command = new CreateReportCommand(email, description, file, latitude, longitude);
            var reportId = await _mediator.Send(command);

            return Created("", new { ReportId = reportId, Message = "Report created" });
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetReport(Guid reportId)
        {
            var query = new GetReportQuery(reportId);
            var report = await _mediator.Send(query);

            if (report == null)
                return NotFound("Report not found");

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
