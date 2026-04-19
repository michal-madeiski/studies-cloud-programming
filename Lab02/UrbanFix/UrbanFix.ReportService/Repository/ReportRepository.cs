using Microsoft.EntityFrameworkCore;
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

        public async Task<Report?> GetByIdAsync(Guid reportId)
        {
            return await _context.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports.ToListAsync();
        }
    }
}
