using MassTransit;
using UrbanFix.AssignmentService.Models;
using UrbanFix.AssignmentService.Repository;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;

namespace UrbanFix.AssignmentService.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _repository;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<AssignmentService> _logger;

        public AssignmentService(IAssignmentRepository repository, IPublishEndpoint publish, ILogger<AssignmentService> logger)
        {
            _repository = repository;
            _publish = publish;
            _logger = logger;
        }

        public async Task<Guid?> AssignTaskAsync(Guid reportId, string? submitterEmail)
        {
            _logger.LogInformation($"[{GetType().Name}] Started assigning task for report - {reportId}");

            var availableTeams = await _repository.GetAvailableTeamsAsync();
            if (!availableTeams.Any())
            {
                _logger.LogError($"[{GetType().Name}] No available technical teams");
                return null;
            }

            var randomTeam = availableTeams.ElementAt(new Random().Next(availableTeams.Count()));

            var assignment = new TaskAssignment
            {
                ReportId = reportId,
                AssignedTeamId = randomTeam.Id,
                Status = TaskAssignmentStatus.New,
                SubmitterEmail = submitterEmail
            };

            await _repository.AddAsync(assignment);
            _logger.LogInformation($"[{GetType().Name}] Created task assignment {assignment.Id} for report - {reportId} to team {randomTeam.Name}");

            randomTeam.IsAvailable = false;
            await _repository.UpdateTeamAsync(randomTeam);
            _logger.LogInformation($"[{GetType().Name}] Team {randomTeam.Name} marked as unavailable");

            await _publish.Publish(new TaskAssignedEvent
            {
                ReportId = reportId,
                TaskAssignmentId = assignment.Id,
                AssignedTeamId = randomTeam.Id,
                SubmitterEmail = submitterEmail,
                AssignedAt = DateTime.UtcNow
            });
            _logger.LogInformation($"[{GetType().Name}] Published TaskAssignedEvent for report - {reportId}");

            return assignment.Id;
        }

        public async Task<TaskAssignment?> GetAssignmentByReportIdAsync(Guid reportId)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving task assignment for report - {reportId}");
            return await _repository.GetByReportIdAsync(reportId);
        }

        public async Task<bool> CompleteTaskAsync(Guid assignmentId)
        {
            _logger.LogInformation($"[{GetType().Name}] Started completing task assignment {assignmentId}");

            var assignment = await _repository.GetByAssignmentIdAsync(assignmentId);
            if (assignment == null)
            {
                _logger.LogError($"[{GetType().Name}] Assignment {assignmentId} not found");
                return false;
            }

            assignment.Status = TaskAssignmentStatus.Completed;
            await _repository.UpdateAsync(assignment);
            _logger.LogInformation($"[{GetType().Name}] Updated assignment {assignmentId} status to Completed");

            if (assignment.AssignedTeam != null)
            {
                assignment.AssignedTeam.IsAvailable = true;
                await _repository.UpdateTeamAsync(assignment.AssignedTeam);
                _logger.LogInformation($"[{GetType().Name}] Team {assignment.AssignedTeam.Name} marked as available again");
            }

            await _publish.Publish(new TaskCompletedEvent
            {
                ReportId = assignment.ReportId,
                TaskAssignmentId = assignment.Id,
                AssignedTeamId = assignment.AssignedTeamId,
                SubmitterEmail = assignment.SubmitterEmail,
                CompletedAt = DateTime.UtcNow
            });
            _logger.LogInformation($"[{GetType().Name}] Published TaskCompletedEvent for report {assignment.ReportId}");

            return true;
        }
    }
}
