namespace SmartSammour.Application.DTOs.Admin
{
    public class UpdatePlanDto
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal ExtraFee { get; set; }

        public decimal StartFrom { get; set; }

        public bool IncludeDomainAnalysis { get; set; }

        public bool IncludeHosting { get; set; }

        public bool IncludeDomainRegistration { get; set; }

        public List<int> ServiceIds { get; set; } = new();
    }
}