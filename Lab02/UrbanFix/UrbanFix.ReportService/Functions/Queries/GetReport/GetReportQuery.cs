using MediatR;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Functions.Queries.GetReport
{
    public class GetReportQuery : IRequest<Report?>
    {
        public Guid ReportId { get; set; }

        public GetReportQuery(Guid reportId)
        {
            ReportId = reportId;
        }
    }
}
