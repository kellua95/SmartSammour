namespace SmartSammour.Application.DTOs.Admin
{
    public class CreateAddOnDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }
        public int ServiceId { get; set; }
    }
}
