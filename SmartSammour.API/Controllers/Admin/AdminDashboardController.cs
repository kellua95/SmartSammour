using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSammour.Application.DTOs.Admin;
using SmartSammour.Core.Entities;
using SmartSammour.Infrastructure.data;

namespace SmartSammour.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminDashboardController(AppDbContext context)
        {
            _context = context; 
        }

        [HttpGet]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
        {
            var dashboard = new AdminDashboardDto
            {
                TotalInquiries = await _context.Inquiries.CountAsync(),

                NewInquiries = await _context.Inquiries
                    .CountAsync(i => i.Status == InquiryStatus.New),

                InProgressInquiries = await _context.Inquiries
                    .CountAsync(i => i.Status == InquiryStatus.InProgress),

                CompletedInquiries = await _context.Inquiries
                    .CountAsync(i => i.Status == InquiryStatus.Completed),

                ActivePlans = await _context.Plans
                    .CountAsync(p => p.IsActive),

                ActiveServices = await _context.Services
                    .CountAsync(s => s.IsActive),

                ActiveAddOns = await _context.AddOns
                    .CountAsync(a => a.IsActive),

                TotalEstimatedValue = await _context.Inquiries
                    .SumAsync(i => i.EstimatedPrice)
            };

            return Ok(dashboard);
        }
    }
}
