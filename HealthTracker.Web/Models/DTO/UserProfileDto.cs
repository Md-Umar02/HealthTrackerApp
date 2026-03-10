namespace HealthTracker.Web.Models.DTO
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }

        public List<HealthMetricDto> HealthMetrics { get; set; }
    }
}
