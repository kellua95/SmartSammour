namespace SmartSammour.Core.Entities
{
    public class Plan
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal ExtraFee { get; set; }
        public bool IncludeDomainAnalysis { get; set; }
        public bool IncludeHosting { get; set; }
        public bool IncludeDomainRegistration { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<PlanService> PlanServices { get; set; } = new List<PlanService>();
    }
}
