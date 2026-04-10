using UrbanFix.AssignmentService.Models;

namespace UrbanFix.AssignmentService.Repository
{
    public interface IAssignmentRepository
    {
        Task AddAsync(TaskAssignment assignment);
        Task<TaskAssignment?> GetByReportIdAsync(Guid reportId);
        Task<TaskAssignment?> GetByAssignmentIdAsync(Guid assignmentId);
        Task<IEnumerable<TechnicalTeam>> GetAllTeamsAsync();
        Task<IEnumerable<TechnicalTeam>> GetAvailableTeamsAsync();
        Task UpdateAsync(TaskAssignment assignment);
        Task UpdateTeamAsync(TechnicalTeam team);
    }
}
