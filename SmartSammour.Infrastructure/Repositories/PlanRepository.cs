using SmartSammour.Core.Entities;
using SmartSammour.Core.Interfaces;
using SmartSammour.Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace SmartSammour.Infrastructure.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly AppDbContext _context;

        public PlanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Plan>> GetAllAsync()
        {
            return await _context.Plans
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<Plan?> GetByIdAsync(int planId)
        {
            return await _context.Plans
                .FirstOrDefaultAsync(p => 
                    p.Id == planId && 
                    p.IsActive);
        }

        public async Task<List<Service>> GetServicesByPlanIdAsync(int planId)
        {
            return await _context.PlanServices
                .Where(ps => ps.PlanId == planId &&
                ps.Plan.IsActive)
                .Select(ps => ps.Service)
                .ToListAsync();
        }

        public async Task<bool> IsServiceAllowedAsync(int planId, int serviceId)
        {
            return await _context.PlanServices
                .AnyAsync(ps => 
                    ps.PlanId == planId &&
                    ps.ServiceId == serviceId);
        }
    }
}
