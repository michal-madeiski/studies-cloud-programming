using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrbanFix.NotificationService.Functions.Queries.GetNotifications;

namespace UrbanFix.NotificationService.Controllers
{
    /// <summary>
    /// Notification service
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get notifications
        /// </summary>
        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetNotifications(Guid reportId)
        {
            var query = new GetNotificationsQuery(reportId);
            var notifications = await _mediator.Send(query);

            if (!notifications.Any())
                return Ok(new List<object>()); // Zwraca pustą listę zamiast 404

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
