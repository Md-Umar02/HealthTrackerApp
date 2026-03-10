using HealthTracker.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Domain.Models
{
    public class HealthMetric : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int MetricTypeId { get; set; }
        public MetricType MetricType { get; set; }

        [Required]
        public double Value { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}
