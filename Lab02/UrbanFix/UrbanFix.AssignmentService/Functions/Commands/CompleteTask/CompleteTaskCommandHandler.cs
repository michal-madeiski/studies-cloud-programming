using MassTransit;
using MediatR;
using UrbanFix.AssignmentService.Repository;
using UrbanFix.Common;
using UrbanFix.Common.Infrastructure;

namespace UrbanFix.AssignmentService.Functions.Commands.CompleteTask
{
    public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
    {
        private readonly IAssignmentRepository _repository;
        private readonly IPublishEndpoint _publish;
        private readonly ILogger<CompleteTaskCommandHandler> _logger;

        public CompleteTaskCommandHandler(
            IAssignmentRepository repository,
            IPublishEndpoint publish,
            ILogger<CompleteTaskCommandHandler> logger)
        {
            _repository = repository;
            _publish = publish;
            _logger = logger;
        }

        public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Started completing task assignment {request.AssignmentId}");

            var assignment = await _repository.GetByAssignmentIdAsync(request.AssignmentId);
            if (assignment == null)
            {
                _logger.LogError($"[{GetType().Name}] Assignment {request.AssignmentId} not found");
                return false;
            }

            assignment.Status = TaskAssignmentStatus.Completed;
            await _repository.UpdateAsync(assignment);
            _logger.LogInformation($"[{GetType().Name}] Updated assignment {request.AssignmentId} status to Completed");

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
