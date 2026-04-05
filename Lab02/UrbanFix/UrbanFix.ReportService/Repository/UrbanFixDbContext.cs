using Microsoft.EntityFrameworkCore;
using System;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Repository
{
    public class UrbanFixDbContext : DbContext
    {
        public UrbanFixDbContext(DbContextOptions<UrbanFixDbContext> options) : base(options) { }

        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Report>().ToTable("reports");
        }
    }
}
