using UrbanFix.TimelineService.Models;
using UrbanFix.TimelineService.Repository;

namespace UrbanFix.TimelineService.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly ITimelineRepository _repository;
        private readonly ILogger<TimelineService> _logger;

        public TimelineService(ITimelineRepository repository, ILogger<TimelineService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Timeline>> GetTimelineByReportIdAsync(Guid reportId)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving timeline for report {reportId}");
            return await _repository.GetByReportIdAsync(reportId);
        }
    }
}
