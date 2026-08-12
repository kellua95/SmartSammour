using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSammour.Application.DTOs;
using SmartSammour.Application.Services;
using SmartSammour.Core.Interfaces;

namespace SmartSammour.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InquiriesController : ControllerBase
    {
        private readonly InquiryService _inquiryService;

        public InquiriesController (InquiryService inquiryService)
        {
            _inquiryService = inquiryService;
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] InquiryRequestDto request)
        {
            try
            {
                var result = await _inquiryService.SubmitInquiryAsync(request);
                return Ok(result);
            }
            catch(ArgumentException ex) 
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromServices] IInquiryRepository repo)
        {
            var inquiries = await repo.GetAllAsync();
            return Ok(inquiries.Select(i => new
            {
                i.Id,
                i.CustomerName,
                i.CustomerEmail,
                i.CustomerPhone,
                i.ProjectDescription,
                ServiceName = i.Service.Name,
                AddOns = i.SelectedAddOns.Select(sa => sa.AddOn.Name),
                i.EstimatedPrice,
                Status = i.Status.ToString(),
                i.CreatedAt
            }));
        }
    }
}
