using HealthTracker.Application.DTO.MetricType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Application.DTO.HealthMetric
{
    public class HealthMetricDto
    {
        public int Id { get; set; }
        //public int UserId { get; set; }
        public string MetricType { get; set; }
        public double Value { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
