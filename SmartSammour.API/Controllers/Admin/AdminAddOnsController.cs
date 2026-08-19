using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSammour.Application.DTOs.Admin;
using SmartSammour.Core.Entities;
using SmartSammour.Infrastructure.data;

namespace SmartSammour.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/addons")]
    [Authorize(Roles = "Admin")]
    public class AdminAddOnsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminAddOnsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var addOns = await _context.AddOns
                .AsNoTracking()
                .OrderBy(a => a.ServiceId)
                .ThenBy(a => a.ExtraPrice)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.ExtraPrice,
                    a.ServiceId,
                    ServiceName = a.Service.Name,
                    a.IsActive
                }).ToListAsync();

            return Ok(addOns);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var addOn = await _context.AddOns
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.ExtraPrice,
                    a.ServiceId,
                    ServiceName = a.Service.Name,
                    a.IsActive
                }).FirstOrDefaultAsync();

            if (addOn == null)
            {
                return NotFound(new
                {
                    message = "Add-On not found."
                });
            }

            return Ok(addOn);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAddOnDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new { message = "Add-On Name is required." });
            }

            if (dto.ExtraPrice < 0)
            {
                return BadRequest(new { message = "Extra Price cannot ne negative." });
            }

            var serviceExists = await _context.Services
                .AnyAsync(s => s.Id == dto.ServiceId);

            if (!serviceExists)
            {
                return BadRequest(new { message = "Selected service is not exist." });
            }

            var name = dto.Name.Trim();

            var duplicate = await _context.AddOns
                .AnyAsync(a =>
                    a.ServiceId == dto.ServiceId &&
                    a.Name.ToLower() == name.ToLower());

            if (duplicate)
            {
                return Conflict(new
                {
                    message = "This add-on already exists for the selected service."
                });
            }

            var addOn = new AddOn
            {
                Name = name,
                ExtraPrice = dto.ExtraPrice,
                ServiceId = dto.ServiceId,
                IsActive = true
            };

            _context.AddOns.Add(addOn);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new {id = addOn.Id},
                new
                {
                    message = "Add-on created successfully.",
                    addOn.Id
                });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateAddOnDto dto)
        {
            var addOn = await _context.AddOns
                .FirstOrDefaultAsync(a => a.Id == id);

            if (addOn == null)
            {
                return NotFound(new { message = "Add-On not Found." });
            }
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new { message = "Add-On Name is required." });
            }
            if (dto.ExtraPrice < 0)
            {
                return BadRequest(new { message = "Add-On Extra price cannot be negative." });
            }
            var serviceExists = await _context.Services
                .AnyAsync(s => s.Id == dto.ServiceId);
            if (!serviceExists) 
            {
                return BadRequest(new { message = "selected service Not exist." });
            }
            var name = dto.Name.Trim();
            var duplicate = await _context.AddOns
                .AnyAsync(a =>
                a.Id != id &&
                a.ServiceId == dto.ServiceId &&
                a.Name.ToLower() == name.ToLower());
            if (duplicate)
            {
                return Conflict(new { message = "Another add-on with this name already exists for the selected service." });
            }

            addOn.Name = name;
            addOn.ExtraPrice = dto.ExtraPrice;
            addOn.ServiceId = dto.ServiceId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Add-on updated successfully." });
        }

        [HttpPatch("{id:int}/active")]
        public async Task<IActionResult> SetActive(
            int id,
            [FromQuery] bool active)
        {
            var addOn = await _context.AddOns
                .FirstOrDefaultAsync(a => a.Id == id);

            if (addOn == null)
            {
                return BadRequest(new { message = "Add-On not found." });
            }

            addOn.IsActive = active;
            await _context.SaveChangesAsync();

            return Ok(new {
                messsage = active ? "Add-On is activate." : "Add-On is deactivate.",
                addOn.Id,
                addOn.IsActive
            });
        }
    }
}
