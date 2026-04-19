using MediatR;
using UrbanFix.Common;

namespace UrbanFix.VerificationService.Functions.Commands.VerifyReport
{
    public class VerifyReportCommand : IRequest<bool>
    {
        public Guid ReportId { get; set; }
        public VerificationDecision Decision { get; set; }
        public string? Comment { get; set; }

        public VerifyReportCommand(Guid reportId, VerificationDecision decision, string? comment)
        {
            ReportId = reportId;
            Decision = decision;
            Comment = comment;
        }
    }
}
