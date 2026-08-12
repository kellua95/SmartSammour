namespace SmartSammour.Application.DTOs
{
    public class InquiryResponseDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public List<string> SelectedAddOnNames { get; set; } = new();
        public decimal EstimatedPrice { get; set; }
        public string message { get; set; } = "Thanks! We'll follow up with your final quote shortly.";
    }
}
