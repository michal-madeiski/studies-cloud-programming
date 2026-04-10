namespace UrbanFix.Common.Infrastructure
{
    public class ReportVerifiedEvent
    {
        public Guid ReportId { get; set; }
        public string? SubmitterEmail { get; set; }
        public VerificationDecision Decision { get; set; }
        public DateTime VerifiedAt { get; set; }
    }
}
