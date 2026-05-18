using Microsoft.EntityFrameworkCore;
using UrbanFix.Common.Pagination;
using UrbanFix.NotificationService.Models;

namespace UrbanFix.NotificationService.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;
        public NotificationRepository(NotificationDbContext context) => _context = context;

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginationResponse<Notification>> GetByReportIdAsync(Guid reportId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Notifications.Where(n => n.ReportId == reportId).OrderBy(n => n.SentAt);
            var totalCount = await query.CountAsync();

            var notifications = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<Notification>
            {
                Items = notifications,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
