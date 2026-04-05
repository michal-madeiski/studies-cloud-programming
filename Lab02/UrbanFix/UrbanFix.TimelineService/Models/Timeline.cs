using UrbanFix.Common;

namespace UrbanFix.TimelineService.Models
{
    public class Timeline
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReportId { get; set; }
        public TaskAssignmentStatus NewStatus { get; set; } = TaskAssignmentStatus.InProgress;
        public string? Description { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
