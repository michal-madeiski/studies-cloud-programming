using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.AssignmentService.Functions.Commands.CompleteTask;
using UrbanFix.AssignmentService.Functions.Queries.GetAssignment;

namespace UrbanFix.AssignmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssignmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetAssignment(Guid reportId)
        {
            var query = new GetAssignmentQuery(reportId);
            var assignment = await _mediator.Send(query);
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

        [HttpPost("{assignmentId}/complete")]
        public async Task<IActionResult> CompleteTask(Guid assignmentId)
        {
            var command = new CompleteTaskCommand(assignmentId);
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound("Assignment not found");

            return Ok(new { Message = "Task completed successfully" });
        }
    }
}

