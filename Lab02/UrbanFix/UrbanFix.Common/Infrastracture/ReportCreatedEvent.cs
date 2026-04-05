namespace UrbanFix.ReportService.Infrastracture
{
    public class ReportCreatedEvent
    {
        public Guid ReportId { get; set; }
        public string? SubmitterEmail { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
