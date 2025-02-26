using System.Net.Mail;
using Infrastructure.IServices;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class EmailNotificationService(ILogger<EmailNotificationService> _logger) : IEmailNotificationService
    {

        public async Task SendPriceAlertEmailAsync(string email, string coinName, decimal currentPrice, decimal targetPrice)
        {
            try
            {
                var subject = $"📢 Price Alert: {coinName} has reached ${currentPrice}!";
                var body = $"Hello,\n\nYour price alert for {coinName} has been triggered. The current price is ${currentPrice}, reaching your target of ${targetPrice}.\n\nBest regards,\nCoinHawk Team";
                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("myemail@gmail.com", "abcd efgh ijkl mnop"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage("your-gmail@gmail.com", email, subject, body);
                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation($"Price alert email sent to {email} for {coinName} at ${currentPrice}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send price alert email.");
            }
        }
    }
}
