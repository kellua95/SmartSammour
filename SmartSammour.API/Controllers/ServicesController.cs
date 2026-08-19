using Microsoft.AspNetCore.Mvc;
using SmartSammour.Application.DTOs;
using SmartSammour.Core.Entities;
using SmartSammour.Core.Interfaces;

namespace SmartSammour.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;

        public ServicesController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _serviceRepository.GetAllAsync();
            return Ok(services.Where(s => s.IsActive).Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null || !service.IsActive) { return NotFound(); }
            return Ok(ToDto(service));
        }

        private static ServiceDto ToDto(Service service) => new()
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            BasePrice = service.BasePrice,
            AddOns = service.AddOns
                .Where(a => a.IsActive)
                .Select(a => new AddOnDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    ExtraPrice = a.ExtraPrice
                }).ToList()
        };
    }
}
