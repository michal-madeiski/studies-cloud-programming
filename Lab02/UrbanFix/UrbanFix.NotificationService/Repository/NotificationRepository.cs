using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Notification>> GetByReportIdAsync(Guid reportId)
        {
            return await Task.FromResult(_context.Notifications.Where(n => n.ReportId == reportId).ToList());
        }
    }
}
