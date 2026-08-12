using SmartSammour.Core.Entities;

namespace SmartSammour.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendInquiryNotificationAsync(Inquiry inquiry);
        Task SendEstimateConfirmationAsync(Inquiry inquiry);
    }
}
