using Microsoft.Extensions.Configuration;
using Resend;
using SmartSammour.Core.Entities;
using SmartSammour.Core.Interfaces;

namespace SmartSammour.Infrastructure.Email
{
    public class ConsoleEmailService : IEmailService
    {

        private readonly IResend _resend;
        private readonly IConfiguration _configuration;

        public ConsoleEmailService(IResend resend, IConfiguration configuration)
        {
            _resend = resend;
            _configuration = configuration;
        }

        public async Task SendInquiryNotificationAsync(Inquiry inquiry)
        {
            var fromEmail = _configuration["Resend:FromEmail"]
                ?? throw new InvalidOperationException("Resend FromEmail is missing.");

            var toEmail = _configuration["Resend:ToEmail"]
                ?? throw new InvalidOperationException("Resend ToEmail is missing.");

            var message = new EmailMessage
            {
                From = $"SmartSammour <{fromEmail}>",
                Subject = $"New SmartSammour Inquiry #{inquiry.Id}",
                HtmlBody = $"""
                    <h2>New SmartSammour Inquiry</h2>

                    <p><strong>Customer:</strong> {inquiry.CustomerName}</p>
                    <p><strong>Email:</strong> {inquiry.CustomerEmail}</p>
                    <p><strong>Phone:</strong> {inquiry.CustomerPhone}</p>

                    <hr />

                    <p><strong>Plan:</strong> {inquiry.Plan?.Name}</p>
                    <p><strong>Service:</strong> {inquiry.Service?.Name}</p>
                    <p><strong>Estimated Price:</strong> {inquiry.EstimatedPrice} JOD</p>

                    <h3>Project Description</h3>
                    <p>{inquiry.ProjectDescription}</p>
                    """
            };

            message.To.Add(toEmail);

            await _resend.EmailSendAsync(message);
        }

        public async Task SendEstimateConfirmationAsync(Inquiry inquiry)
        {
            var fromEmail = _configuration["Resend:FromEmail"]
                ?? throw new InvalidOperationException("Resend FromEmail is missing.");

            var message = new EmailMessage
            {
                From = $"SmartSammour <{fromEmail}>",
                Subject = "Your SmartSammour Project Estimate",
                HtmlBody = $"""
                    <h2>Hello {inquiry.CustomerName},</h2>

                    <p>Thank you for contacting SmartSammour.</p>

                    <p>Your initial estimated project price is:</p>

                    <h2>{inquiry.EstimatedPrice} JOD</h2>

                    <p>
                        This is an initial estimate based on your selected requirements.
                        We will review your project and contact you with the final quote.
                    </p>

                    <p>Best regards,<br/>SmartSammour Team</p>
                    """
            };

            message.To.Add(inquiry.CustomerEmail);

            await _resend.EmailSendAsync(message);
        }
    }
}
