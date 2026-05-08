using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.VerificationService.Functions.Commands.VerifyReport;
using UrbanFix.VerificationService.Functions.Queries.GetVerification;
using UrbanFix.VerificationService.Models;
using UrbanFix.VerificationService.Repository;

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
        private readonly IVerificationRepository _repository;

        public VerificationsController(IMediator mediator, IVerificationRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        /// <summary>
        /// Get all verifications with pagination support
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllVerifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _repository.GetAllVerificationsAsync(pageNumber, pageSize);
            return Ok(new
            {
                items = result.Items.Select(v => new
                {
                    verificationId = v.Id,
                    reportId = v.ReportId,
                    submitterEmail = v.SubmitterEmail,
                    officeWorkerId = v.OfficeWorkerId,
                    decision = v.Decision,
                    comment = v.Comment,
                    verifiedAt = v.VerifiedAt
                }).ToList(),
                pageNumber = result.PageNumber,
                pageSize = result.PageSize,
                totalCount = result.TotalCount,
                totalPages = result.TotalPages,
                hasPreviousPage = result.HasPreviousPage,
                hasNextPage = result.HasNextPage
            });
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
        [Authorize(Roles = "Admin")]
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
