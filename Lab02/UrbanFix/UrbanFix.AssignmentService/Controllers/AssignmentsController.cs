using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.AssignmentService.Functions.Commands.CompleteTask;
using UrbanFix.AssignmentService.Functions.Queries.GetAssignment;
using UrbanFix.AssignmentService.Models;
using UrbanFix.AssignmentService.Repository;

namespace UrbanFix.AssignmentService.Controllers
{
    /// <summary>
    /// Task assignment service
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAssignmentRepository _repository;

        public AssignmentsController(IMediator mediator, IAssignmentRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        /// <summary>
        /// Get all assignments with pagination support
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllAssignments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _repository.GetAllAssignmentsAsync(pageNumber, pageSize);
            return Ok(new
            {
                items = result.Items.Select(a => new
                {
                    assignmentId = a.Id,
                    reportId = a.ReportId,
                    assignedTeamId = a.AssignedTeamId,
                    teamName = a.AssignedTeam?.Name,
                    teamAvailable = a.AssignedTeam?.IsAvailable,
                    status = a.Status
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
        /// Get assignment details
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetAssignment(Guid reportId)
        {
            var query = new GetAssignmentQuery(reportId);
            var assignment = await _mediator.Send(query);
            if (assignment == null)
                return NotFound(new { message = "Assignment not found" });

            return Ok(new
            {
                assignmentId = assignment.Id,
                reportId = assignment.ReportId,
                assignedTeamId = assignment.AssignedTeamId,
                teamName = assignment.AssignedTeam?.Name,
                teamAvailable = assignment.AssignedTeam?.IsAvailable,
                status = assignment.Status
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{assignmentId}/complete")]
        public async Task<IActionResult> CompleteAssignment(Guid assignmentId)
        {
            var command = new CompleteTaskCommand(assignmentId);
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = "Assignment not found" });

            return NoContent();
        }

        /// <summary>
        /// Get all technical teams sorted by name
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("teams/all")]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _repository.GetAllTeamsAsync();
            return Ok(teams.Select(t => new
            {
                id = t.Id,
                name = t.Name,
                isAvailable = t.IsAvailable
            }));
        }

        /// <summary>
        /// Get available technical teams sorted by name
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("teams/available")]
        public async Task<IActionResult> GetAvailableTeams()
        {
            var teams = await _repository.GetAvailableTeamsPaginatedAsync();
            return Ok(teams.Select(t => new
            {
                id = t.Id,
                name = t.Name,
                isAvailable = t.IsAvailable
            }));
        }

        /// <summary>
        /// Get unavailable technical teams sorted by name
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("teams/unavailable")]
        public async Task<IActionResult> GetUnavailableTeams()
        {
            var teams = await _repository.GetUnavailableTeamsAsync();
            return Ok(teams.Select(t => new
            {
                id = t.Id,
                name = t.Name,
                isAvailable = t.IsAvailable
            }));
        }

        /// <summary>
        /// Get technical team by ID
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("teams/{teamId}")]
        public async Task<IActionResult> GetTeam(Guid teamId)
        {
            var team = await _repository.GetTeamByIdAsync(teamId);
            if (team == null)
                return NotFound(new { message = "Team not found" });

            return Ok(new
            {
                id = team.Id,
                name = team.Name,
                isAvailable = team.IsAvailable
            });
        }

        /// <summary>
        /// Create new technical team
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("teams")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Team name is required" });

            var team = new TechnicalTeam
            {
                Name = request.Name,
                IsAvailable = request.IsAvailable ?? true
            };

            await _repository.AddTeamAsync(team);

            return CreatedAtAction(nameof(GetTeam), new { teamId = team.Id }, new
            {
                id = team.Id,
                name = team.Name,
                isAvailable = team.IsAvailable
            });
        }

        /// <summary>
        /// Update technical team
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPatch("teams/{teamId}")]
        public async Task<IActionResult> UpdateTeam(Guid teamId, [FromBody] UpdateTeamRequest request)
        {
            var team = await _repository.GetTeamByIdAsync(teamId);
            if (team == null)
                return NotFound(new { message = "Team not found" });

            if (!string.IsNullOrWhiteSpace(request.Name))
                team.Name = request.Name;

            if (request.IsAvailable.HasValue)
                team.IsAvailable = request.IsAvailable.Value;

            await _repository.UpdateTeamAsync(team);

            return Ok(new
            {
                id = team.Id,
                name = team.Name,
                isAvailable = team.IsAvailable
            });
        }

        /// <summary>
        /// Delete technical team
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("teams/{teamId}")]
        public async Task<IActionResult> DeleteTeam(Guid teamId)
        {
            var team = await _repository.GetTeamByIdAsync(teamId);
            if (team == null)
                return NotFound(new { message = "Team not found" });

            await _repository.DeleteTeamAsync(teamId);
            return NoContent();
        }
    }

    public class CreateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool? IsAvailable { get; set; }
    }

    public class UpdateTeamRequest
    {
        public string? Name { get; set; }
        public bool? IsAvailable { get; set; }
    }
}

