using SmartSammour.Core.Entities;

namespace SmartSammour.Core.Interfaces
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetAllAsync();
        Task<Service?> GetByIdAsync(int id);
    }
}
