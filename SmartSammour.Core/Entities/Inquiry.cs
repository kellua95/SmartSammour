namespace SmartSammour.Core.Entities
{

    public enum inquiryStatus { New, Contacted, Quoted, Closed }
    public class Inquiry
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

        public ICollection<InquiryAddOn> SelectedAddOns { get; set; } = new List<InquiryAddOn>();

        public decimal EstimatedPrice { get; set; }
        public inquiryStatus Status { get; set; } = inquiryStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class InquiryAddOn
    {
        public int InquiryId { get; set; }
        public Inquiry Inquiry { get; set; } = null!;
        public int AddOnId { get; set; }
        public AddOn AddOn { get; set; } = null!;
    }
}
