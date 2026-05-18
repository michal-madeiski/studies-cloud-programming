using Microsoft.EntityFrameworkCore;
using UrbanFix.AssignmentService.Models;
using UrbanFix.Common;
using UrbanFix.Common.Pagination;

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

        /// <summary>
        /// Get all assignments with pagination support
        /// </summary>
        public async Task<PaginationResponse<TaskAssignment>> GetAllAssignmentsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.TaskAssignments.Include(t => t.AssignedTeam);
            var totalCount = await query.CountAsync();

            var assignments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<TaskAssignment>
            {
                Items = assignments,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Get all technical teams sorted by name
        /// </summary>
        public async Task<IEnumerable<TechnicalTeam>> GetAllTeamsAsync()
        {
            return await _context.TechnicalTeams.OrderBy(t => t.Name).ToListAsync();
        }

        /// <summary>
        /// Get available technical teams sorted by name
        /// </summary>
        public async Task<IEnumerable<TechnicalTeam>> GetAvailableTeamsPaginatedAsync()
        {
            return await _context.TechnicalTeams.Where(t => t.IsAvailable).OrderBy(t => t.Name).ToListAsync();
        }

        /// <summary>
        /// Get unavailable technical teams sorted by name
        /// </summary>
        public async Task<IEnumerable<TechnicalTeam>> GetUnavailableTeamsAsync()
        {
            return await _context.TechnicalTeams.Where(t => !t.IsAvailable).OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<TechnicalTeam?> GetTeamByIdAsync(Guid teamId)
        {
            return await _context.TechnicalTeams.FirstOrDefaultAsync(t => t.Id == teamId);
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

        public async Task AddTeamAsync(TechnicalTeam team)
        {
            await _context.TechnicalTeams.AddAsync(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeamAsync(Guid teamId)
        {
            var team = await _context.TechnicalTeams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team != null)
            {
                _context.TechnicalTeams.Remove(team);
                await _context.SaveChangesAsync();
            }
        }
    }
}
