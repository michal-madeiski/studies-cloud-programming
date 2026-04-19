namespace UrbanFix.ReportService.Models
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? SubmitterEmail { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public string Description { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // S3 Metadata
        public string? S3BucketName { get; set; }
        public string? S3ObjectKey { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

