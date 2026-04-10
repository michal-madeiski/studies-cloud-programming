using UrbanFix.Common;

namespace UrbanFix.VerificationService.Models
{
    public class VerifyRequest
    {
        public VerificationDecision Decision { get; set; }
        public string? Comment { get; set; }
    }
}
