namespace UrbanFix.NotificationService.Models
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReportId { get; set; }
        public string? RecipientEmail { get; set; }
        public string MessageBody { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
