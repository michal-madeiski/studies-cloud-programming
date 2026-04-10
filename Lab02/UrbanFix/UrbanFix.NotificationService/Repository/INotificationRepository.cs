using UrbanFix.NotificationService.Models;

namespace UrbanFix.NotificationService.Repository
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetByReportIdAsync(Guid reportId);
    }
}
