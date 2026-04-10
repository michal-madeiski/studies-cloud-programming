namespace UrbanFix.Common.Infrastructure
{
    public class NotificationSentEvent
    {
        public Guid ReportId { get; set; }
        public string? RecipientEmail { get; set; }
        public TaskAssignmentStatus Status { get; set; }
        public DateTime SentAt { get; set; }
    }
}
