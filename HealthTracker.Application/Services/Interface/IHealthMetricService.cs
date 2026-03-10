using HealthTracker.Application.DTO.HealthMetric;
using HealthTracker.Application.InputModels;
using HealthTracker.Application.ViewModels;
using HealthTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Application.Services.Interface
{
    public interface IHealthMetricService
    {
        Task<PaginationVM<HealthMetricDto>> GetPagination(PaginationInputModel pagination);
        Task<IEnumerable<HealthMetricDto>> GetAllAsync();
        Task<IEnumerable<HealthMetricDto>> GetAllByFilterAsync(int? metricTypeId, int? userId);
        Task<HealthMetricDto> CreateAsync(string identityUser, CreateHealthMetricDto createHealthMetricDto);
        Task UpdateAsync(UpdateHealthMetricDto updateHealthMetricDto);
        Task<HealthMetricDto?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task <IEnumerable<HealthMetricDto>>GetByIdentityUserAsync(string identityUserId);
    }
}
