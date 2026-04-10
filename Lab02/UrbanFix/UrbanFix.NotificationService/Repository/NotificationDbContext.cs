using Microsoft.EntityFrameworkCore;
using UrbanFix.NotificationService.Models;

namespace UrbanFix.NotificationService.Repository
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Notification>().ToTable("notifications");
        }
    }
}
