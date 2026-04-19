using MediatR;
using UrbanFix.TimelineService.Models;

namespace UrbanFix.TimelineService.Functions.Queries.GetTimeline
{
    public class GetTimelineQuery : IRequest<IEnumerable<Timeline>>
    {
        public Guid ReportId { get; set; }

        public GetTimelineQuery(Guid reportId)
        {
            ReportId = reportId;
        }
    }
}
