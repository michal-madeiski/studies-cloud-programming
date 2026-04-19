using MediatR;

namespace UrbanFix.AssignmentService.Functions.Commands.CompleteTask
{
    public class CompleteTaskCommand : IRequest<bool>
    {
        public Guid AssignmentId { get; set; }

        public CompleteTaskCommand(Guid assignmentId)
        {
            AssignmentId = assignmentId;
        }
    }
}
