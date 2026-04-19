using MediatR;
using UrbanFix.AssignmentService.Models;
using UrbanFix.AssignmentService.Repository;

namespace UrbanFix.AssignmentService.Functions.Queries.GetAssignment
{
    public class GetAssignmentQueryHandler : IRequestHandler<GetAssignmentQuery, TaskAssignment?>
    {
        private readonly IAssignmentRepository _repository;
        private readonly ILogger<GetAssignmentQueryHandler> _logger;

        public GetAssignmentQueryHandler(IAssignmentRepository repository, ILogger<GetAssignmentQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<TaskAssignment?> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving task assignment for report - {request.ReportId}");
            return await _repository.GetByReportIdAsync(request.ReportId);
        }
    }
}
