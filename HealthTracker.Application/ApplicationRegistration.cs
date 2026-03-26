using HealthTracker.Application.Common;
using HealthTracker.Application.Services;
using HealthTracker.Application.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Application
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped(typeof(IPaginationService<,>), typeof(PaginationService<,>));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IHealthMetricService, HealthMetricService>();
            services.AddScoped<IMetricTypeService, MetricTypeService>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddHttpClient<IAIService, AIService>();

            return services;
        }
    }
}
