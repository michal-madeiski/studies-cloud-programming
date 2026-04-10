using Microsoft.EntityFrameworkCore;
using UrbanFix.TimelineService.Models;

namespace UrbanFix.TimelineService.Repository
{
    public class TimelineDbContext : DbContext
    {
        public TimelineDbContext(DbContextOptions<TimelineDbContext> options) : base(options) { }

        public DbSet<Timeline> Timelines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Timeline>().ToTable("timelines");
            modelBuilder.Entity<Timeline>()
                .Property(t => t.NewStatus)
                .HasConversion<string>();
        }
    }
}
