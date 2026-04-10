using UrbanFix.AssignmentService.Models;

namespace UrbanFix.AssignmentService.Services
{
    public interface IAssignmentService
    {
        Task<Guid?> AssignTaskAsync(Guid reportId, string? submitterEmail);
        Task<TaskAssignment?> GetAssignmentByReportIdAsync(Guid reportId);
        Task<bool> CompleteTaskAsync(Guid assignmentId);
    }
}
