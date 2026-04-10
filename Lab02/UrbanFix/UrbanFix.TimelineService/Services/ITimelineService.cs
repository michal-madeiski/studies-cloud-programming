using UrbanFix.TimelineService.Models;

namespace UrbanFix.TimelineService.Services
{
    public interface ITimelineService
    {
        Task<IEnumerable<Timeline>> GetTimelineByReportIdAsync(Guid reportId);
    }
}
