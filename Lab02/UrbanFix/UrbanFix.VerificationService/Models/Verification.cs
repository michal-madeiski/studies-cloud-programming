using UrbanFix.Common;

namespace UrbanFix.VerificationService.Models
{
    public class Verification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReportId { get; set; }
        public string? SubmitterEmail { get; set; }
        public string OfficeWorkerId { get; set; } = "Admin_01";
        public VerificationDecision Decision { get; set; } = VerificationDecision.Pending;
        public string? Comment { get; set; }
        public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
    }
}
