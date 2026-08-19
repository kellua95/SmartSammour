namespace SmartSammour.Core.Entities
{
    public class AddOn
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }

        public int ServiceId { get; set; }
        public bool IsActive { get; set; } = true;

        public Service Service { get; set; } = null!;
    }
}
