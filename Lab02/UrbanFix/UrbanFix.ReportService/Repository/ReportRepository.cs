using Microsoft.EntityFrameworkCore;
using UrbanFix.Common;
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

        public async Task UpdateAsync(Report report)
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginationResponse<Report>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool sortDescending = false, DateTime? from = null, DateTime? to = null, ReportStatus? status = null)
        {
            IQueryable<Report> query = _context.Reports;

            if (from.HasValue)
                query = query.Where(r => r.UploadedAt >= from.Value);
            if (to.HasValue)
                query = query.Where(r => r.UploadedAt <= to.Value);
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            query = sortDescending
                ? query.OrderByDescending(r => r.UploadedAt)
                : query.OrderBy(r => r.UploadedAt);

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
