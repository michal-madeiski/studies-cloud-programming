namespace UrbanFix.Common.Infrastructure
{
    public class TaskCompletedEvent
    {
        public Guid ReportId { get; set; }
        public Guid TaskAssignmentId { get; set; }
        public Guid AssignedTeamId { get; set; }
        public string? SubmitterEmail { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
