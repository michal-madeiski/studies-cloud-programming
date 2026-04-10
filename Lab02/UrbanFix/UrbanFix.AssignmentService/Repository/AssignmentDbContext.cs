using Microsoft.EntityFrameworkCore;
using UrbanFix.AssignmentService.Models;

namespace UrbanFix.AssignmentService.Repository
{
    public class AssignmentDbContext : DbContext
    {
        public AssignmentDbContext(DbContextOptions<AssignmentDbContext> options) : base(options) { }

        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<TechnicalTeam> TechnicalTeams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TaskAssignment>().ToTable("task_assignments");
            modelBuilder.Entity<TechnicalTeam>().ToTable("technical_teams");

            modelBuilder.Entity<TaskAssignment>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.AssignedTeam)
                .WithMany()
                .HasForeignKey(t => t.AssignedTeamId);

            var teamIds = new[]
            {
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Guid.Parse("10000000-0000-0000-0000-000000000007"),
                Guid.Parse("10000000-0000-0000-0000-000000000008"),
                Guid.Parse("10000000-0000-0000-0000-000000000009"),
                Guid.Parse("10000000-0000-0000-0000-000000000010")
            };

            modelBuilder.Entity<TechnicalTeam>().HasData(
                new TechnicalTeam { Id = teamIds[0], Name = "Team_01", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[1], Name = "Team_02", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[2], Name = "Team_03", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[3], Name = "Team_04", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[4], Name = "Team_05", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[5], Name = "Team_06", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[6], Name = "Team_07", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[7], Name = "Team_08", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[8], Name = "Team_09", IsAvailable = true },
                new TechnicalTeam { Id = teamIds[9], Name = "Team_10", IsAvailable = true }
            );
        }
    }
}
