using Microsoft.EntityFrameworkCore;
using UrbanFix.Common.Pagination;
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

        public async Task<PaginationResponse<Report>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Reports;
            var totalCount = await query.CountAsync();

            var reports = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<Report>
            {
                Items = reports,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
