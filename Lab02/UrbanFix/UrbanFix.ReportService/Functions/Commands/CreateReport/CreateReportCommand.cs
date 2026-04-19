using MediatR;

namespace UrbanFix.ReportService.Functions.Commands.CreateReport
{
    public class CreateReportCommand : IRequest<Guid>
    {
        public string Email { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public CreateReportCommand(string email, string description, IFormFile file, double latitude, double longitude)
        {
            Email = email;
            Description = description;
            File = file;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
