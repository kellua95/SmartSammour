namespace SmartSammour.Application.DTOs.Admin
{
    public class AdminDashboardDto
    {
        public int TotalInquiries { get; set; }
        public int NewInquiries { get; set; }
        public int InProgressInquiries { get; set; }
        public int CompletedInquiries { get; set; }

        public int ActivePlans { get; set; }
        public int ActiveServices { get; set; }
        public int ActiveAddOns { get; set; }

        public Decimal TotalEstimatedValue { get; set; }
    }
}
