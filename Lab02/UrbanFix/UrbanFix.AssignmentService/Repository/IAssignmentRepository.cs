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
        Task<IEnumerable<TechnicalTeam>> GetAllTeamsAsync();
        Task<IEnumerable<TechnicalTeam>> GetAvailableTeamsPaginatedAsync();
        Task<IEnumerable<TechnicalTeam>> GetUnavailableTeamsAsync();
        Task<TechnicalTeam?> GetTeamByIdAsync(Guid teamId);
        Task<IEnumerable<TechnicalTeam>> GetAvailableTeamsAsync();
        Task UpdateAsync(TaskAssignment assignment);
        Task UpdateTeamAsync(TechnicalTeam team);
        Task AddTeamAsync(TechnicalTeam team);
        Task DeleteTeamAsync(Guid teamId);
    }
}
