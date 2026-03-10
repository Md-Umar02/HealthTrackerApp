namespace HealthTracker.Web.Models.DTO
{
    public class HealthMetricDto
    {
        public int Id { get; set; }
        public string MetricType { get; set; }
        public double Value { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
