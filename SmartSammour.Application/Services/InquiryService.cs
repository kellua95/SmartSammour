using SmartSammour.Application.DTOs;
using SmartSammour.Core.Entities;
using SmartSammour.Core.Interfaces;

namespace SmartSammour.Application.Services
{
    public class InquiryService
    {
        private readonly IInquiryRepository _inquiryRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IEmailService _emailService;
        private readonly PricingEngine _pricingEngine;
        private readonly IPlanRepository _planRepository;

        public InquiryService(
            IInquiryRepository inquiryRepository,
            IServiceRepository serviceReposetory,
            IEmailService emailService,
            PricingEngine pricingEngine,
            IPlanRepository planRepository
            )
        {
            _inquiryRepository = inquiryRepository;
            _serviceRepository = serviceReposetory;
            _emailService = emailService;
            _pricingEngine = pricingEngine;
            _planRepository = planRepository;
        }

        public async Task<InquiryResponseDto> SubmitInquiryAsync(InquiryRequestDto request)
        {
            var plan = await _planRepository.GetByIdAsync(request.PlanId)
                ?? throw new ArgumentException("plan not found");

            var service = await _serviceRepository.GetByIdAsync(request.ServiceId)
                ?? throw new ArgumentException("service not found");

            var selectedAddOns = service.AddOns
                .Where(a => request.SelectedAddOnIds.Contains(a.Id))
                .ToList();

            var estmatedPrice = _pricingEngine.Calculate(service, plan, selectedAddOns);

            var inquiry = new Inquiry
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                ProjectDescription = request.ProjectDescription,

                ServiceId = service.Id,
                PlanId = plan.Id,

                EstimatedPrice = estmatedPrice,
                
                SelectedAddOns = selectedAddOns
                    .Select(a => new InquiryAddOn { AddOnId = a.Id, })
                    .ToList()
            };

            var saved = await _inquiryRepository.AddAsync(inquiry);

            await _emailService.SendInquiryNotificationAsync(saved);
            await _emailService.SendEstimateConfirmationAsync(saved);

            return new InquiryResponseDto
            {
                Id = saved.Id,
                ServiceName = service.Name,
                SelectedAddOnNames = selectedAddOns.Select(a => a.Name).ToList(),
                EstimatedPrice = estmatedPrice
            };
        }
    }
}
