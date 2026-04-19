using MediatR;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Functions.Queries.GetAllReports
{
    public class GetAllReportsQuery : IRequest<IEnumerable<Report>>
    {
    }
}
