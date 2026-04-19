using MediatR;
using UrbanFix.TimelineService.Models;
using UrbanFix.TimelineService.Repository;

namespace UrbanFix.TimelineService.Functions.Queries.GetTimeline
{
    public class GetTimelineQueryHandler : IRequestHandler<GetTimelineQuery, IEnumerable<Timeline>>
    {
        private readonly ITimelineRepository _repository;
        private readonly ILogger<GetTimelineQueryHandler> _logger;

        public GetTimelineQueryHandler(ITimelineRepository repository, ILogger<GetTimelineQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Timeline>> Handle(GetTimelineQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving timeline for report {request.ReportId}");
            return await _repository.GetByReportIdAsync(request.ReportId);
        }
    }
}
