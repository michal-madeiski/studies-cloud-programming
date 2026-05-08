using UrbanFix.Common.Pagination;
using UrbanFix.TimelineService.Models;

namespace UrbanFix.TimelineService.Repository
{
    public interface ITimelineRepository
    {
        Task AddAsync(Timeline timeline);
        Task<PaginationResponse<Timeline>> GetByReportIdAsync(Guid reportId, int pageNumber = 1, int pageSize = 10);
    }
}
