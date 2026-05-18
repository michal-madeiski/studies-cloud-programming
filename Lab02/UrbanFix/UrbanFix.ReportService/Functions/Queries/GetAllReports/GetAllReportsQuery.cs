using MediatR;
using UrbanFix.Common;
using UrbanFix.Common.Pagination;
using UrbanFix.ReportService.Models;

namespace UrbanFix.ReportService.Functions.Queries.GetAllReports
{
    public class GetAllReportsQuery : IRequest<PaginationResponse<Report>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool SortDescending { get; set; } = false;
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public ReportStatus? Status { get; set; }

        public GetAllReportsQuery(int pageNumber = 1, int pageSize = 10, bool sortDescending = false, DateTime? from = null, DateTime? to = null, ReportStatus? status = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SortDescending = sortDescending;
            From = from;
            To = to;
            Status = status;
        }
    }
}
