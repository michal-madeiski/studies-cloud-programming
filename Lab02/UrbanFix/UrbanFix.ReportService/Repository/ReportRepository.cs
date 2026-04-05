using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly UrbanFixDbContext _context;
        public ReportRepository(UrbanFixDbContext context) => _context = context;

        public async Task AddAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
        }
    }
}
