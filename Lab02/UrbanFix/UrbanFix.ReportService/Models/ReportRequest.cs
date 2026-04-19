namespace UrbanFix.ReportService.Models
{
    public class ReportRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
