using Microsoft.EntityFrameworkCore;
using UrbanFix.AssignmentService.Models;

namespace UrbanFix.AssignmentService.Repository
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AssignmentDbContext _context;
        public AssignmentRepository(AssignmentDbContext context) => _context = context;

        public async Task AddAsync(TaskAssignment assignment)
        {
            await _context.TaskAssignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<TaskAssignment?> GetByReportIdAsync(Guid reportId)
        {
            return await _context.TaskAssignments
                .Include(t => t.AssignedTeam)
                .FirstOrDefaultAsync(t => t.ReportId == reportId);
        }

        public async Task<TaskAssignment?> GetByAssignmentIdAsync(Guid assignmentId)
        {
            return await _context.TaskAssignments
                .Include(t => t.AssignedTeam)
                .FirstOrDefaultAsync(t => t.Id == assignmentId);
        }

        public async Task<IEnumerable<TechnicalTeam>> GetAllTeamsAsync()
        {
            return await _context.TechnicalTeams.ToListAsync();
        }

        public async Task<IEnumerable<TechnicalTeam>> GetAvailableTeamsAsync()
        {
            return await _context.TechnicalTeams.Where(t => t.IsAvailable).ToListAsync();
        }

        public async Task UpdateAsync(TaskAssignment assignment)
        {
            _context.TaskAssignments.Update(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTeamAsync(TechnicalTeam team)
        {
            _context.TechnicalTeams.Update(team);
            await _context.SaveChangesAsync();
        }
    }
}
