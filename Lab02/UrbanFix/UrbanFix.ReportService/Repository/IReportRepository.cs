using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Repository
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
    }
}
