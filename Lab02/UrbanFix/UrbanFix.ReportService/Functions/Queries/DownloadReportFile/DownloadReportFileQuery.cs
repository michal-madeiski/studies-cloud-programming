using MediatR;

namespace UrbanFix.ReportService.Functions.Queries.DownloadReportFile
{
    public class DownloadReportFileQuery : IRequest<DownloadReportFileResult?>
    {
        public Guid ReportId { get; set; }

        public DownloadReportFileQuery(Guid reportId)
        {
            ReportId = reportId;
        }
    }

    public class DownloadReportFileResult
    {
        public Models.Report Report { get; set; }
        public Stream FileStream { get; set; }

        public DownloadReportFileResult(Models.Report report, Stream fileStream)
        {
            Report = report;
            FileStream = fileStream;
        }
    }
}
