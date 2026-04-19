using MediatR;

namespace UrbanFix.VerificationService.Functions.Queries.GetVerification
{
    public class GetVerificationQuery : IRequest<Guid?>
    {
        public Guid ReportId { get; set; }

        public GetVerificationQuery(Guid reportId)
        {
            ReportId = reportId;
        }
    }
}
