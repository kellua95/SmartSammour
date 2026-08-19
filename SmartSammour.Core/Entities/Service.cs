namespace SmartSammour.Core.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<AddOn> AddOns { get; set; } = new List<AddOn>();
        public ICollection<PlanService> PlanServices { get; set; } = new List<PlanService>();
    }
}
