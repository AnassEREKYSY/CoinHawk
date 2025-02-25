using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Dtos;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.IServices;
using CoinGecko.Clients;

namespace Infrastructure.Services
{
    public class EmailNotificationService(ILogger<EmailNotificationService> _logger) : IEmailNotificationService
    {

        public async Task SendPriceAlertEmailAsync(string email, string coinName, decimal currentPrice, decimal targetPrice)
        {
            try
            {
                var subject = $"📢 Price Alert: {coinName} has reached ${currentPrice}!";
                var body = $"Hello,\n\nYour price alert for {coinName} has been triggered. The current price is ${currentPrice}, reaching your target of ${targetPrice}.\n\nBest regards,\CoinHawk Team";
                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Use 587 for TLS (recommended) or 465 for SSL
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
