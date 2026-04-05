namespace UrbanFix.ReportService.Services
{
    public interface IReportService
    {
        Task<Guid> CreateReportAsync(string email, string description, IFormFile file);
    }
}
