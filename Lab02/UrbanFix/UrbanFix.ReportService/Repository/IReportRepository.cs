using UrbanFix.Common;
using UrbanFix.Common.Pagination;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Repository
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<Report?> GetByIdAsync(Guid reportId);
        Task UpdateAsync(Report report);
        Task<PaginationResponse<Report>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool sortDescending = false, DateTime? from = null, DateTime? to = null, ReportStatus? status = null);
    }
}
