using Microsoft.AspNetCore.Mvc;
using UrbanFix.NotificationService.Services;

namespace UrbanFix.NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetNotifications(Guid reportId)
        {
            var notifications = await _notificationService.GetNotificationsByReportIdAsync(reportId);
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
