using MediatR;
using UrbanFix.NotificationService.Models;

namespace UrbanFix.NotificationService.Functions.Queries.GetNotifications
{
    public class GetNotificationsQuery : IRequest<IEnumerable<Notification>>
    {
        public Guid ReportId { get; set; }

        public GetNotificationsQuery(Guid reportId)
        {
            ReportId = reportId;
        }
    }
}
