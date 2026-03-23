using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Web.Models.DTO
{
    public class UpdateHealthMetricDto
    {
        public int Id { get; set; }
        [Required]
        public int MetricTypeId { get; set; }
        public double Value { get; set; }
    }
}
