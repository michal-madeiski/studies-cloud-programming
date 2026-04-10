using UrbanFix.Common;
using UrbanFix.NotificationService.Models;

namespace UrbanFix.NotificationService.Services
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(Guid reportId, string? recipientEmail, TaskAssignmentStatus status, string description);
        Task<IEnumerable<Notification>> GetNotificationsByReportIdAsync(Guid reportId);
    }
}
