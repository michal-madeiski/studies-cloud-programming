namespace UrbanFix.Common.Infrastructure
{
    public class StatusChangedEvent
    {
        public Guid ReportId { get; set; }
        public TaskAssignmentStatus NewStatus { get; set; }
        public string? Description { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
