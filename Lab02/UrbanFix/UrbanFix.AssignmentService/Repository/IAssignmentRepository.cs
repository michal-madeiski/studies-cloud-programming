using UrbanFix.AssignmentService.Models;
using UrbanFix.Common.Pagination;

namespace UrbanFix.AssignmentService.Repository
{
    public interface IAssignmentRepository
    {
        Task AddAsync(TaskAssignment assignment);
        Task<TaskAssignment?> GetByReportIdAsync(Guid reportId);
        Task<TaskAssignment?> GetByAssignmentIdAsync(Guid assignmentId);
        Task<PaginationResponse<TaskAssignment>> GetAllAssignmentsAsync(int pageNumber = 1, int pageSize = 10);
        Task<PaginationResponse<TechnicalTeam>> GetAllTeamsAsync(int pageNumber = 1, int pageSize = 10);
        Task<TechnicalTeam?> GetTeamByIdAsync(Guid teamId);
        Task<IEnumerable<TechnicalTeam>> GetAvailableTeamsAsync();
        Task<IEnumerable<TaskAssignment>> GetSomeCompletedAssignmentsAsync(int count);
        Task UpdateAsync(TaskAssignment assignment);
        Task UpdateTeamAsync(TechnicalTeam team);
        Task AddTeamAsync(TechnicalTeam team);
        Task DeleteTeamAsync(Guid teamId);
    }
}
