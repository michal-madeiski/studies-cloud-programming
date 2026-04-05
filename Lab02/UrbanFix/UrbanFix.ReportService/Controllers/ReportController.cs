using Microsoft.AspNetCore.Mvc;
using UrbanFix.ReportService.Services;

namespace UrbanFix.ReportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromForm] string email, [FromForm] string description, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is necessary");

            if (email == null || email == string.Empty)
                return BadRequest("Email is necessary");

            var reportId = await _reportService.CreateReportAsync(email, description, file);

            return Created("", new { ReportId = reportId, Message = "Report created" });
        }
    }
}