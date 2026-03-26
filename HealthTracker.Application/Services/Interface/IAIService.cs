using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Application.Services.Interface
{
    public interface IAIService
    {
        Task<string> GetHealthInsightsAsync(string prompt);
    }
}
