using MediatR;
using UrbanFix.NotificationService.Models;
using UrbanFix.NotificationService.Repository;

namespace UrbanFix.NotificationService.Functions.Queries.GetNotifications
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<Notification>>
    {
        private readonly INotificationRepository _repository;
        private readonly ILogger<GetNotificationsQueryHandler> _logger;

        public GetNotificationsQueryHandler(INotificationRepository repository, ILogger<GetNotificationsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Notification>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{GetType().Name}] Retrieving notifications for report {request.ReportId}");
            return await _repository.GetByReportIdAsync(request.ReportId);
        }
    }
}
