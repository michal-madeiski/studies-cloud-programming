using UrbanFix.Common.Pagination;
using UrbanFix.VerificationService.Models;

namespace UrbanFix.VerificationService.Repository
{
    public interface IVerificationRepository
    {
        Task AddAsync(Verification verification);
        Task<Verification?> GetByReportIdAsync(Guid reportId);
        Task<PaginationResponse<Verification>> GetAllVerificationsAsync(int pageNumber = 1, int pageSize = 10);
        Task UpdateAsync(Verification verification);
    }
}
