namespace SmartSammour.Core.Entities
{
    public class PlanService
    {
        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}
