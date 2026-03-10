using HealthTracker.Domain.Contracts;
using HealthTracker.Domain.Models;
using HealthTracker.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Infrastructure.Repositoies
{
    public class HealthMetricRepository : GenericRepository<HealthMetric>, IHealthMetricRepository
    {
        public HealthMetricRepository(ApplicationDbContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<HealthMetric>> GetAllAsync()
        {
            return await _context.HealthMetrics
                .Include(hm => hm.MetricType)
                .Include(hm => hm.User)
                .ToListAsync();
        }
        public override async Task<HealthMetric?> GetByIdAsync(int id)
        {
            return await _context.HealthMetrics
                .Include(hm => hm.MetricType)
                .Include(hm => hm.User)
                .FirstOrDefaultAsync(hm => hm.Id == id);
        }

        public async Task<IEnumerable<HealthMetric>> GetByUserIdAsync(int userId)
        {
            return await _context.HealthMetrics
                .Include(h => h.MetricType)
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.RecordedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(HealthMetric entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
