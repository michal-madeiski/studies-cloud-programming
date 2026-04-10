using UrbanFix.Common;

namespace UrbanFix.AssignmentService.Models
{
    public class TaskAssignment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReportId { get; set; }
        public Guid AssignedTeamId { get; set; }
        public string? SubmitterEmail { get; set; }
        public TaskAssignmentStatus Status { get; set; } = TaskAssignmentStatus.New;

        public TechnicalTeam? AssignedTeam { get; set; }
    }
}
