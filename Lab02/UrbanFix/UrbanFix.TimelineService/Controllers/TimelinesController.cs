using Microsoft.AspNetCore.Mvc;
using UrbanFix.TimelineService.Services;

namespace UrbanFix.TimelineService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimelinesController : ControllerBase
    {
        private readonly ITimelineService _timelineService;

        public TimelinesController(ITimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetTimeline(Guid reportId)
        {
            var timeline = await _timelineService.GetTimelineByReportIdAsync(reportId);
            if (!timeline.Any())
                return NotFound("Timeline not found");

            return Ok(timeline.OrderBy(t => t.OccurredAt).Select(t => new
            {
                t.Id,
                t.ReportId,
                t.NewStatus,
                t.Description,
                t.OccurredAt
            }));
        }
    }
}
