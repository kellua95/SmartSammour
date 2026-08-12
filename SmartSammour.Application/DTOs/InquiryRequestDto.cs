using System.ComponentModel.DataAnnotations;

namespace SmartSammour.Application.DTOs
{
    public class InquiryRequestDto
    {
        [Required, MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        [Phone]
        public string? CustomerPhone { get; set; }

        [Required , MaxLength(2000)]
        public string ProjectDescription { get; set; } = string.Empty;

        public int ServiceId { get; set; }
        public List<int> SelectedAddOnIds { get; set; } = new();

        public int PlanId { get; set; }
    }
}
