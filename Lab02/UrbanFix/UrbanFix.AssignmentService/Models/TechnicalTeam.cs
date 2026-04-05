namespace UrbanFix.AssignmentService.Models
{
    public class TechnicalTeam
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Team_01";
        public bool IsAvailable { get; set; } = true;
    }
}
