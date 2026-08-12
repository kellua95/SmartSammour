using Microsoft.AspNetCore.Mvc;
using SmartSammour.Application.DTOs;
using SmartSammour.Core.Interfaces;

namespace SmartSammour.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly IPlanRepository _planRepository;

        public PlansController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<PlanDto>>> GetPlans()
        {
            var plans = await _planRepository.GetAllAsync();

            var result = plans.Select(p => new PlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ExtraFee = p.ExtraFee,
                IncludesDomainAnalysis = p.IncludeDomainAnalysis,
                IncludesHosting = p.IncludeHosting,
                IncludesDomainRegistration = p.IncludeDomainRegistration,
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{planId}/services")]
        public async Task<ActionResult<List<ServiceDto>>> GetServicesByPlan(int planId)
        {

            var plan = await _planRepository.GetByIdAsync(planId);

            if (plan == null)
            {
                return NotFound($"Plan not found.");
            }
            
            var services = await _planRepository.GetServicesByPlanIdAsync(planId);

            var result = services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                BasePrice = s.BasePrice
            }).ToList();

            return Ok(result);
        }
    }
}
