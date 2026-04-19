using MassTransit;
using MediatR;
using UrbanFix.AssignmentService.Models;
using UrbanFix.AssignmentService.Repository;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;

namespace UrbanFix.AssignmentService.Functions.Commands.AssignTask
{
    public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, Guid?>
    {
        private readonly IAssignmentRepository _repository;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<AssignTaskCommandHandler> _logger;

        public AssignTaskCommandHandler(
            IAssignmentRepository repository,
            IPublishEndpoint publish,
            ILogger<AssignTaskCommandHandler> logger)
        {
            _repository = repository;
            _publish = publish;
            _logger = logger;
        }

        public async Task<Guid?> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Started assigning task for report - {request.ReportId}");

            var availableTeams = await _repository.GetAvailableTeamsAsync();
            if (!availableTeams.Any())
            {
                _logger.LogError($"[{GetType().Name}] No available technical teams");
                return null;
            }

            var randomTeam = availableTeams.ElementAt(new Random().Next(availableTeams.Count()));

            var assignment = new TaskAssignment
            {
                ReportId = request.ReportId,
                AssignedTeamId = randomTeam.Id,
                Status = TaskAssignmentStatus.New,
                SubmitterEmail = request.SubmitterEmail
            };

            await _repository.AddAsync(assignment);
            _logger.LogInformation($"[{GetType().Name}] Created task assignment {assignment.Id} for report - {request.ReportId} to team {randomTeam.Name}");

            randomTeam.IsAvailable = false;
            await _repository.UpdateTeamAsync(randomTeam);
            _logger.LogInformation($"[{GetType().Name}] Team {randomTeam.Name} marked as unavailable");

            await _publish.Publish(new TaskAssignedEvent
            {
                ReportId = request.ReportId,
                TaskAssignmentId = assignment.Id,
                AssignedTeamId = randomTeam.Id,
                SubmitterEmail = request.SubmitterEmail,
                AssignedAt = DateTime.UtcNow
            });
            _logger.LogInformation($"[{GetType().Name}] Published TaskAssignedEvent for report - {request.ReportId}");

            return assignment.Id;
        }
    }
}
