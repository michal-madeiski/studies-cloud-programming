using Microsoft.EntityFrameworkCore;
using UrbanFix.VerificationService.Models;

namespace UrbanFix.VerificationService.Repository
{
    public class VerificationDbContext : DbContext
    {
        public VerificationDbContext(DbContextOptions<VerificationDbContext> options) : base(options) { }

        public DbSet<Verification> Verifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Verification>().ToTable("verifications");
            modelBuilder.Entity<Verification>()
                .Property(v => v.Decision)
                .HasConversion<string>();
        }
    }
}
