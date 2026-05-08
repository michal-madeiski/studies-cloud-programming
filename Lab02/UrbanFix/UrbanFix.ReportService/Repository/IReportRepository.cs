using UrbanFix.Common.Pagination;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Repository
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<Report?> GetByIdAsync(Guid reportId);
        Task<PaginationResponse<Report>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    }
}
