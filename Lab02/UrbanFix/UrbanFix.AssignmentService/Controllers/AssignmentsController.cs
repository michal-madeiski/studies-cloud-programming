using Microsoft.AspNetCore.Mvc;
using UrbanFix.AssignmentService.Services;

namespace UrbanFix.AssignmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetAssignment(Guid reportId)
        {
            var assignment = await _assignmentService.GetAssignmentByReportIdAsync(reportId);
            if (assignment == null)
                return NotFound("Assignment not found");

            return Ok(new
            {
                AssignmentId = assignment.Id,
                ReportId = assignment.ReportId,
                AssignedTeamId = assignment.AssignedTeamId,
                TeamName = assignment.AssignedTeam?.Name,
                TeamAvailable = assignment.AssignedTeam?.IsAvailable,
                Status = assignment.Status
            });
        }

        [HttpPost("{reportId}/complete")]
        public async Task<IActionResult> CompleteTask(Guid reportId)
        {
            var result = await _assignmentService.CompleteTaskAsync(reportId);
            if (!result)
                return NotFound("Assignment not found");

            return Ok(new { Message = "Task completed successfully" });
        }
    }
}
