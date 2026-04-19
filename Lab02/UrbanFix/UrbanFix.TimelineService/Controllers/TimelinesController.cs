using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.TimelineService.Functions.Queries.GetTimeline;

namespace UrbanFix.TimelineService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimelinesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TimelinesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetTimeline(Guid reportId)
        {
            var query = new GetTimelineQuery(reportId);
            var timeline = await _mediator.Send(query);
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
