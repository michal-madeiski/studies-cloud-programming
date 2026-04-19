using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.VerificationService.Functions.Commands.VerifyReport;
using UrbanFix.VerificationService.Functions.Queries.GetVerification;
using UrbanFix.VerificationService.Models;

namespace UrbanFix.VerificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VerificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetVerification(Guid reportId)
        {
            var query = new GetVerificationQuery(reportId);
            var verificationId = await _mediator.Send(query);
            if (verificationId == null)
                return NotFound("Verification not found");

            return Ok(new { VerificationId = verificationId });
        }

        [HttpPost("{reportId}/verify")]
        public async Task<IActionResult> VerifyReport(Guid reportId, [FromBody] VerifyRequest request)
        {
            var command = new VerifyReportCommand(reportId, request.Decision, request.Comment);
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound("Verification not found");

            return Ok(new { Message = "Report verified successfully" });
        }
    }
}

