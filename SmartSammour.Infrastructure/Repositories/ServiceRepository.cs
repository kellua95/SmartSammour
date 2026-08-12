using Microsoft.EntityFrameworkCore;
using SmartSammour.Core.Entities;
using SmartSammour.Core.Interfaces;
using SmartSammour.Infrastructure.data;

namespace SmartSammour.Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;
        public ServiceRepository(AppDbContext context) => _context = context;

        public async Task<List<Service>> GetAllAsync() => 
            await _context.Services.Include(s => s.AddOns).ToListAsync();
        public async Task<Service?> GetByIdAsync(int id) =>
            await _context.Services.Include(s => s.AddOns).FirstOrDefaultAsync(s => s.Id == id);
    }
}
