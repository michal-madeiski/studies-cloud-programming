using UrbanFix.Common;

namespace UrbanFix.VerificationService.Services
{
    public interface IVerificationService
    {
        Task<Guid?> GetVerificationByReportIdAsync(Guid reportId);
        Task<bool> VerifyReportAsync(Guid reportId, VerificationDecision decision, string? comment);
    }
}
