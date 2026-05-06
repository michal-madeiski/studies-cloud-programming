using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.VerificationService.Functions.Commands.VerifyReport;
using UrbanFix.VerificationService.Functions.Queries.GetVerification;
using UrbanFix.VerificationService.Models;

namespace UrbanFix.VerificationService.Controllers
{
    /// <summary>
    /// Report verification service
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VerificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get verification status
        /// </summary>
        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetVerification(Guid reportId)
        {
            var query = new GetVerificationQuery(reportId);
            var verificationId = await _mediator.Send(query);
            if (verificationId == null)
                return NotFound(new { message = "Verification not found" });

            return Ok(new { verificationId });
        }

        /// <summary>
        /// Verify report
        /// </summary>
        [HttpPatch("{reportId}")]
        public async Task<IActionResult> UpdateVerification(Guid reportId, [FromBody] VerifyRequest request)
        {
            var command = new VerifyReportCommand(reportId, request.Decision, request.Comment);
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = "Verification not found" });

            return NoContent();
        }
    }
}

