using HealthTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Domain.Contracts
{
    public interface IHealthMetricRepository : IGenericRepository<HealthMetric>
    {
        Task<IEnumerable<HealthMetric>> GetByUserIdAsync(int userId);
        Task UpdateAsync(HealthMetric entity);
    }
}
