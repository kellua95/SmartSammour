namespace SmartSammour.Application.DTOs
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public List<AddOnDto> AddOns { get; set; } = new();
    }
}
