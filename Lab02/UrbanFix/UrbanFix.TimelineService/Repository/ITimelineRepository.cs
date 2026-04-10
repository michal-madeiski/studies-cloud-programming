using UrbanFix.TimelineService.Models;

namespace UrbanFix.TimelineService.Repository
{
    public interface ITimelineRepository
    {
        Task AddAsync(Timeline timeline);
        Task<IEnumerable<Timeline>> GetByReportIdAsync(Guid reportId);
    }
}
