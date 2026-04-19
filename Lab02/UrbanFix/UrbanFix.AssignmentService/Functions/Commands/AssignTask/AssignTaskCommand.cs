using MediatR;

namespace UrbanFix.AssignmentService.Functions.Commands.AssignTask
{
    public class AssignTaskCommand : IRequest<Guid?>
    {
        public Guid ReportId { get; set; }
        public string? SubmitterEmail { get; set; }

        public AssignTaskCommand(Guid reportId, string? submitterEmail)
        {
            ReportId = reportId;
            SubmitterEmail = submitterEmail;
        }
    }
}
