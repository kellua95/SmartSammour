using SmartSammour.Core.Entities;

namespace SmartSammour.Core.Interfaces
{
    public interface IInquiryRepository
    {
        Task<Inquiry> AddAsync(Inquiry inquiry);
        Task<Inquiry?> GetByIdAsync(int id);
        Task<List<Inquiry>> GetAllAsync();
    }
}
