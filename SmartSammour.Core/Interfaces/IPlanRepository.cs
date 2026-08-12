using SmartSammour.Core.Entities;

namespace SmartSammour.Core.Interfaces 
{
    public interface IPlanRepository
    {
        Task<Plan?> GetByIdAsync(int planId);
        Task<List<Plan>> GetAllAsync();

        Task<List<Service>> GetServicesByPlanIdAsync(int planId);
        Task<bool> IsServiceAllowedAsync(int planId, int serviceId);
    }
}
