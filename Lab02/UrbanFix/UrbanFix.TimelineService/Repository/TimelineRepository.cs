using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Timeline>> GetByReportIdAsync(Guid reportId)
        {
            return await Task.FromResult(_context.Timelines.Where(t => t.ReportId == reportId).ToList());
        }
    }
}
