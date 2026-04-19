using MediatR;
using UrbanFix.AssignmentService.Models;

namespace UrbanFix.AssignmentService.Functions.Queries.GetAssignment
{
    public class GetAssignmentQuery : IRequest<TaskAssignment?>
    {
        public Guid ReportId { get; set; }

        public GetAssignmentQuery(Guid reportId)
        {
            ReportId = reportId;
        }
    }
}
