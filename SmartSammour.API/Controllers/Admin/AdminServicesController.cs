using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSammour.Application.DTOs.Admin;
using SmartSammour.Core.Entities;
using SmartSammour.Infrastructure.data;

namespace SmartSammour.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/services")]
    [Authorize(Roles = "Admin")]
    public class AdminServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminServicesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var services = await _context.Services
                .AsNoTracking()
                .OrderBy(s => s.BasePrice)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.BasePrice,
                    s.IsActive,

                    AddOnsCount = s.AddOns.Count,

                    ActiveAddOnsCount = s.AddOns.Count(a => a.IsActive),
                })
                .ToListAsync();

            return Ok(services);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            var service = await _context.Services
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.BasePrice,
                    s.IsActive,

                    AddOnsCount = s.AddOns
                        .OrderBy(a => a.ExtraPrice)
                        .Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.ExtraPrice,
                            a.IsActive
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service not found"
                });
            }

            return Ok(service);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateServiceDto dto)
        {
            if(string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new
                {
                    message = "Service name is required"
                });
            }

            if(dto.BasePrice < 0)
            {
                return BadRequest(new
                {
                    message = "Base price cannot be negative"
                });
            }

            var name = dto.Name.Trim();

            var existe = await _context.Services
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());

            if (existe)
            {
                return BadRequest(new
                {
                    message = "Service with this name already exists."
                });
            }

            var service = new Service
            {
                Name = name,
                Description = dto.Description?.Trim() ?? string.Empty,
                BasePrice = dto.BasePrice,
                IsActive = true
            };

            _context.Services.Add(service);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = service.Id },
                new
                {
                    message = "Service created successfully",
                    service.Id
                });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, UpdateServiceDto dto)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service not found"
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new
                {
                    message = "Service name is required"
                });
            }

            if (dto.BasePrice < 0)
            {
                return BadRequest(new
                {
                    message = "Base price cannot be negative"
                });
            }

            var name = dto.Name.Trim();
            
            var duplicate = await _context.Services
                .AnyAsync(s => 
                s.Id != id &&
                s.Name.ToLower() == name.ToLower());

            if (duplicate)
            {
                return Conflict(new
                {
                    message = "Service with this name already exists."
                });
            }

            service.Name = name;
            service.Description = 
                dto.Description?.Trim() ?? string.Empty;
            service.BasePrice = dto.BasePrice;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Service updated successfully"
            });
        }

        [HttpPatch("{id:int}/active")]
        public async Task<ActionResult> SetActive(
            int id,
            [FromQuery] bool active)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service not found."
                });
            }

            service.IsActive = active;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = active ? "Service activated." : "Service deactivated.",
                service.Id,
                service.IsActive
            });
        }
    }
}
