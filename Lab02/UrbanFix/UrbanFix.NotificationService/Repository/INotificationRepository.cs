using UrbanFix.Common.Pagination;
using UrbanFix.NotificationService.Models;

namespace UrbanFix.NotificationService.Repository
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<PaginationResponse<Notification>> GetByReportIdAsync(Guid reportId, int pageNumber = 1, int pageSize = 10);
    }
}
