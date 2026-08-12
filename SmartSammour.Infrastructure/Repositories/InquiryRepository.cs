using Microsoft.EntityFrameworkCore;
using SmartSammour.Core.Entities;
using SmartSammour.Core.Interfaces;
using SmartSammour.Infrastructure.data;

namespace SmartSammour.Infrastructure.Repositories
{
    public class InquiryRepository : IInquiryRepository
    {
        private readonly AppDbContext _context;
        public InquiryRepository(AppDbContext context) => _context = context;

        public async Task<Inquiry> AddAsync(Inquiry inquiry)
        {
            _context.Inquiries.Add(inquiry);
            await _context.SaveChangesAsync();
            return inquiry;
        }

        public async Task<List<Inquiry>> GetAllAsync() =>
            await _context.Inquiries
                .Include(i => i.Service)
                .Include(i => i.SelectedAddOns).ThenInclude(ia => ia.AddOn)
                .ToListAsync();

        public async Task<Inquiry?> GetByIdAsync(int id) =>
            await _context.Inquiries
                .Include(i => i.Service)
                .Include(i => i.SelectedAddOns).ThenInclude(ia => ia.AddOn)
                .FirstOrDefaultAsync(i => i.Id == id);
    }
}
