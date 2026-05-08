using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AssignmentsController(IMediator mediator, IAssignmentRepository repository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _mediator = mediator;
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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

        /// <summary>
        /// Get some completed assignments with report details (photo path and description)
        /// </summary>
        [HttpGet("completed")]
        public async Task<IActionResult> GetSomeCompletedAssignments([FromQuery] int count = 10)
        {
            if (count < 1 || count > 100)
                return BadRequest(new { message = "Count must be between 1 and 100" });

            var completedAssignments = await _repository.GetSomeCompletedAssignmentsAsync(count);

            var client = _httpClientFactory.CreateClient();
            var reportServiceUrl = Environment.GetEnvironmentVariable("REPORT_SERVICE_URL")
                ?? _configuration.GetValue<string>("Services:ReportService:Url")
                ?? "http://localhost:5201";

            var result = new List<object>();

            foreach (var assignment in completedAssignments)
            {
                try
                {
                    var reportResponse = await client.GetAsync($"{reportServiceUrl}/api/reports/{assignment.ReportId}");

                    if (reportResponse.IsSuccessStatusCode)
                    {
                        var jsonContent = await reportResponse.Content.ReadAsStringAsync();
                        using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                        {
                            var root = doc.RootElement;

                            result.Add(new
                            {
                                assignmentId = assignment.Id,
                                reportId = assignment.ReportId,
                                teamName = assignment.AssignedTeam?.Name,
                                photoPath = root.TryGetProperty("s3ObjectKey", out var s3Key) ? s3Key.GetString() : null,
                                description = root.TryGetProperty("description", out var desc) ? desc.GetString() : null,
                                fileName = root.TryGetProperty("fileName", out var fn) ? fn.GetString() : null
                            });
                        }
                    }
                }
                catch
                {
                    result.Add(new
                    {
                        assignmentId = assignment.Id,
                        reportId = assignment.ReportId,
                        teamName = assignment.AssignedTeam?.Name,
                        photoPath = (string?)null,
                        description = (string?)null,
                        fileName = (string?)null
                    });
                }
            }

            return Ok(result);
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
        /// Get all technical teams with pagination support
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("teams/all")]
        public async Task<IActionResult> GetAllTeams([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _repository.GetAllTeamsAsync(pageNumber, pageSize);
            return Ok(new
            {
                items = result.Items.Select(t => new
                {
                    id = t.Id,
                    name = t.Name,
                    isAvailable = t.IsAvailable
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

