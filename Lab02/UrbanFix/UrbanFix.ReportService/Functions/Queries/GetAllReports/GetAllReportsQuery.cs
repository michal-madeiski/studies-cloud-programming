using MediatR;
using UrbanFix.Common.Pagination;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Functions.Queries.GetAllReports
{
    public class GetAllReportsQuery : IRequest<PaginationResponse<Report>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetAllReportsQuery(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
