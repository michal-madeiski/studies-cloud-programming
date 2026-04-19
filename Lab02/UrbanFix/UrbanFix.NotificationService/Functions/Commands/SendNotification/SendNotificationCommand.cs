using MediatR;
using UrbanFix.Common;

namespace UrbanFix.NotificationService.Functions.Commands.SendNotification
{
    public class SendNotificationCommand : IRequest<bool>
    {
        public Guid ReportId { get; set; }
        public string? RecipientEmail { get; set; }
        public TaskAssignmentStatus Status { get; set; }
        public string Description { get; set; }

        public SendNotificationCommand(Guid reportId, string? recipientEmail, TaskAssignmentStatus status, string description)
        {
            ReportId = reportId;
            RecipientEmail = recipientEmail;
            Status = status;
            Description = description;
        }
    }
}
