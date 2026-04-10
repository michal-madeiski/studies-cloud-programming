using UrbanFix.VerificationService.Models;

namespace UrbanFix.VerificationService.Repository
{
    public interface IVerificationRepository
    {
        Task AddAsync(Verification verification);
        Task<Verification?> GetByReportIdAsync(Guid reportId);
        Task UpdateAsync(Verification verification);
    }
}
