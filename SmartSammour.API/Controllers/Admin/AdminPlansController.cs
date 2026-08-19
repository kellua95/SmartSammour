using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSammour.Core.Entities;
using SmartSammour.Infrastructure.data;
using SmartSammour.Application.DTOs.Admin;

namespace SmartSammour.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/plans")]
    [Authorize(Roles = "Admin")]
    public class AdminPlansController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminPlansController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plan>>> GetAll()
        {
            var plans = await _context.Plans
                .AsNoTracking()
                .OrderBy(p => p.StartFrom)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.ExtraFee,
                    p.StartFrom,
                    p.IncludeDomainAnalysis,
                    p.IncludeHosting,
                    p.IncludeDomainRegistration,
                    p.IsActive,

                    Services = p.PlanServices
                        .Select(ps => new
                        {
                            ps.Service.Id,
                            ps.Service.Name,
                            ps.Service.Description,
                            ps.Service.BasePrice,
                            ps.Service.IsActive
                        }).ToList()
                }).ToListAsync();

            return Ok(plans);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Plan>> GetById(int id)
        {
            var plan = await _context.Plans
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.ExtraFee,
                    p.StartFrom,
                    p.IncludeDomainAnalysis,
                    p.IncludeHosting,
                    p.IncludeDomainRegistration,
                    p.IsActive,

                    Services = p.PlanServices
                        .Select(ps => new
                        {
                            ps.Service.Id,
                            ps.Service.Name,
                            ps.Service.Description,
                            ps.Service.BasePrice,
                            ps.Service.IsActive
                        }).ToList()
                }).ToListAsync();

            if (plan == null)
            {
                return NotFound(new
                {
                    message = $"Plan not found."
                });
            }

            return Ok(plan);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreatePlanDto dto)
        {
            var exists = await _context.Plans.AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower());

            if (exists)
            {
                return Conflict(new
                {
                    message = $"Plan with name '{dto.Name}' already exists."
                });
            }
            var plan = new Plan
            {
                Name = dto.Name,
                Description = dto.Description,
                ExtraFee = dto.ExtraFee,
                StartFrom = dto.StartFrom,
                IncludeDomainAnalysis = dto.IncludeDomainAnalysis,
                IncludeHosting = dto.IncludeHosting,
                IncludeDomainRegistration = dto.IncludeDomainRegistration,
                IsActive = true
            };

            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();

            if (dto.ServiceIds != null && dto.ServiceIds.Count > 0)
            {
                var validServiceIds = await _context.Services
                    .Where(s => dto.ServiceIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync();

                foreach (var serviceId in validServiceIds)
                {
                    _context.PlanServices.Add(new PlanService
                    {
                        PlanId = plan.Id,
                        ServiceId = serviceId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = plan.Id },
                new
                {
                    message = $"Plan '{plan.Name}' created successfully.",
                    plan.Id
                });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, UpdatePlanDto dto)
        {
            var plan = await _context.Plans
                .Include(p => p.PlanServices)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
            {
                return NotFound(new
                {
                    message = $"Plan not found."
                });
            }

            var duplicateName = await _context.Plans
                .AnyAsync(p =>
                    p.Id != id &&
                    p.Name.ToLower() == dto.Name.ToLower());

            if (duplicateName)
            {
                return Conflict(new
                {
                    message = $"Another plan with name '{dto.Name}' already exists."
                });
            }

            plan.Name = dto.Name.Trim();
            plan.Description = dto.Description.Trim();
            plan.ExtraFee = dto.ExtraFee;
            plan.StartFrom = dto.StartFrom;
            plan.IncludeDomainAnalysis = dto.IncludeDomainAnalysis;
            plan.IncludeHosting = dto.IncludeHosting;
            plan.IncludeDomainRegistration = dto.IncludeDomainRegistration;

            // Update services
            if (dto.ServiceIds != null)
            {
                _context.PlanServices.RemoveRange(plan.PlanServices);

                var validServiceIds = await _context.Services
                .Where(s => dto.ServiceIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

                foreach (var serviceId in validServiceIds)
                {
                    _context.PlanServices.Add(new PlanService
                    {
                        PlanId = plan.Id,
                        ServiceId = serviceId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = $"Plan '{plan.Name}' updated successfully."
            });
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> SetActive(
            int id,
            [FromBody] bool active)
        {
            var plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return NotFound(new
                {
                    message = $"Plan not found."
                });
            }
            plan.IsActive = active;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = $"Plan '{plan.Name}' is now {(active ? "active" : "inactive")}.",
                plan.Id,
                plan.IsActive
            });
        }
    }
}
