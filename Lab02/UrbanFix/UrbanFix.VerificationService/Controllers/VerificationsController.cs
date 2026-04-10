using Microsoft.AspNetCore.Mvc;
using UrbanFix.VerificationService.Models;
using UrbanFix.VerificationService.Services;

namespace UrbanFix.VerificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationsController : ControllerBase
    {
        private readonly IVerificationService _verificationService;

        public VerificationsController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetVerification(Guid reportId)
        {
            var verificationId = await _verificationService.GetVerificationByReportIdAsync(reportId);
            if (verificationId == null)
                return NotFound("Verification not found");

            return Ok(new { VerificationId = verificationId });
        }

        [HttpPost("{reportId}/verify")]
        public async Task<IActionResult> VerifyReport(Guid reportId, [FromBody] VerifyRequest request)
        {
            var result = await _verificationService.VerifyReportAsync(reportId, request.Decision, request.Comment);
            if (!result)
                return NotFound("Verification not found");

            return Ok(new { Message = "Report verified successfully" });
        }
    }
}
