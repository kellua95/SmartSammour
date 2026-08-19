using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSammour.Application.DTOs.Admin;
using SmartSammour.Core.Entities;
using SmartSammour.Infrastructure.data;

namespace SmartSammour.API.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminInquiriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminInquiriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminInquiryDto>>> GetAll(
            [FromQuery] InquiryStatus? status = null,
            [FromQuery] bool? isActive = null)
        {
            var query = _context.Inquiries
                .AsNoTracking()
                .Include(i => i.Service)
                .Include(i => i.Plan)
                .Include(i => i.SelectedAddOns)
                    .ThenInclude(ia => ia.AddOn)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(i => i.IsActive == isActive.Value);
            }

            var inquiries = await query
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new AdminInquiryDto
                {
                    id = i.Id,
                    CustomerName = i.CustomerName,
                    CustomerEmail = i.CustomerEmail,
                    CustomerPhone = i.CustomerPhone,
                    ProjectDescription = i.ProjectDescription,

                    ServiceId = i.ServiceId,
                    ServiceName = i.Service.Name,

                    PlanId = i.PlanId,
                    PlanName = i.Plan.Name,

                    EstimatedPrice = i.EstimatedPrice,
                    Status = i.Status,
                    AdminNotes = i.AdminNotes,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    IsActive = i.IsActive,

                    AddOns = i.SelectedAddOns.Select(x => x.AddOn.Name).ToList()
                }).ToListAsync();

            return Ok(inquiries);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdminInquiryDto>> GetById([FromRoute] int id)
        {
            var inquiry = await _context.Inquiries
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new AdminInquiryDto
                {
                    id = i.Id,
                    CustomerName = i.CustomerName,
                    CustomerEmail = i.CustomerEmail,
                    CustomerPhone = i.CustomerPhone,
                    ProjectDescription = i.ProjectDescription,

                    ServiceId = i.ServiceId,
                    ServiceName = i.Service.Name,

                    PlanId = i.PlanId,
                    PlanName = i.Plan.Name,

                    EstimatedPrice = i.EstimatedPrice,
                    Status = i.Status,
                    AdminNotes = i.AdminNotes,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    IsActive = i.IsActive,

                    AddOns = i.SelectedAddOns.Select(x => x.AddOn.Name).ToList()
                }).FirstOrDefaultAsync();

            if (inquiry == null)
            {
                return NotFound(new { message = "Inquiry not found." });
            }
            return Ok(inquiry);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult> UpdateStatus(
            int id,
            UpdateInquiryStatusDto dto)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry == null)
            {
                return NotFound(new { message = "Inquiry not found." });
            }

            inquiry.Status = dto.Status;
            inquiry.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Inquiry status updated.",
                inquiry.Id,
                inquiry.Status,
                inquiry.UpdatedAt
            });
        }

        [HttpPatch("{id:int}/notes")]
        public async Task<ActionResult> UpdateNotes(
            int id,
            UpdateInquiryNotesDto dto)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);

            if (inquiry == null)
            {
                return NotFound(new { message = "Inquiry not found." });
            }

            inquiry.AdminNotes = dto.AdminNotes?.Trim();
            inquiry.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Inquiry admin notes updated.",
                inquiry.Id,
                inquiry.AdminNotes,
                inquiry.UpdatedAt
            });
        }
    }
}
