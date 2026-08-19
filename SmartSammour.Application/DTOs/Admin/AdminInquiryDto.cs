using SmartSammour.Core.Entities;

namespace SmartSammour.Application.DTOs.Admin
{
    public class AdminInquiryDto
    {
        public int id {  get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;

        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;

        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;

        public decimal EstimatedPrice { get; set; }
        public InquiryStatus Status { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public List<string> AddOns { get; set; } = new();
    }
}
