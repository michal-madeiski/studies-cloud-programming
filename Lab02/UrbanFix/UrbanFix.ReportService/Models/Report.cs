namespace UrbanFix.ReportService.Models
{
    public class Report
    {
        public Guid Id {  get; set; } = Guid.NewGuid();
        public string? SubmitterEmail { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "NEW";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
