namespace UrbanFix.Common.Infrastructure
{
    public class TaskAssignedEvent
    {
        public Guid ReportId { get; set; }
        public Guid TaskAssignmentId { get; set; }
        public Guid AssignedTeamId { get; set; }
        public string? SubmitterEmail { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
