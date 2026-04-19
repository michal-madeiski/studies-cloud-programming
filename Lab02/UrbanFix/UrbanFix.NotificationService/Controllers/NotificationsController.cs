using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.NotificationService.Functions.Queries.GetNotifications;

namespace UrbanFix.NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetNotifications(Guid reportId)
        {
            var query = new GetNotificationsQuery(reportId);
            var notifications = await _mediator.Send(query);
            if (!notifications.Any())
                return NotFound("Notifications not found");

            return Ok(notifications.Select(n => new
            {
                n.Id,
                n.ReportId,
                n.RecipientEmail,
                n.MessageBody,
                n.SentAt
            }));
        }
    }
}
