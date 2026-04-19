using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Repository
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<Report?> GetByIdAsync(Guid reportId);
        Task<IEnumerable<Report>> GetAllAsync();
    }
}
