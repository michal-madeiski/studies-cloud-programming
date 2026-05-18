using Microsoft.EntityFrameworkCore;
using UrbanFix.Common.Pagination;
using UrbanFix.TimelineService.Models;

namespace UrbanFix.TimelineService.Repository
{
    public class TimelineRepository : ITimelineRepository
    {
        private readonly TimelineDbContext _context;
        public TimelineRepository(TimelineDbContext context) => _context = context;

        public async Task AddAsync(Timeline timeline)
        {
            await _context.Timelines.AddAsync(timeline);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginationResponse<Timeline>> GetByReportIdAsync(Guid reportId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Timelines.Where(t => t.ReportId == reportId).OrderBy(t => t.OccurredAt);
            var totalCount = await query.CountAsync();

            var timelines = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<Timeline>
            {
                Items = timelines,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
