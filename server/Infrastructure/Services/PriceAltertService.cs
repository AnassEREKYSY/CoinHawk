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
    public class PriceAltertService(ApplicationDbContext _context,ICoinService _coinService) : IPriceAlertService
    {

        public async Task<PriceAlert> CreatePriceAlertAsync(string userToken, int coinId, decimal targetPrice)
        {
            var userId = ExtractUserIdFromToken(token);
            var priceAlert = new PriceAlert
            {
                UserId = userId,
                CoinId = coinId,
                TargetPrice = targetPrice,
                AlertSetAt = DateTime.UtcNow,
                IsNotified = false
            };

            _context.PriceAlerts.Add(priceAlert);
            await _context.SaveChangesAsync();

            return priceAlert;
        }

        public async Task<IEnumerable<PriceAlertDto>> GetAllAlertsForUserAsync(string token)
        {
            var userId = ExtractUserIdFromToken(token);
            var alerts = await _context.PriceAlerts
                .Include(a => a.Coin)
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var result = new List<PriceAlertDto>();

            foreach (var alert in alerts)
            {
                var coinInfo = await _coinService.GetCoinInfoAsync(alert.Coin.Name, 7);
                var alertDto = new PriceAlertDto
                {
                    Id = alert.Id,
                    UserId = alert.UserId,
                    CoinId = alert.CoinId.ToString(),
                    TargetPrice = alert.TargetPrice,
                    AlertSetAt = alert.AlertSetAt,
                    AlertTriggeredAt = alert.AlertTriggeredAt,
                    IsNotified = alert.IsNotified,
                    Coin = coinInfo
                };

                result.Add(alertDto);
            }

            return result;
        }

        public async Task<PriceAlertDto> GetAlertForUserAsync(int alertId, string token)
        {
            var userId = ExtractUserIdFromToken(token);

            var alert = await _context.PriceAlerts
                .Include(a => a.Coin)
                .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

            if (alert == null)
            {
                throw new Exception("Alert not found or you are not authorized to access this alert.");
            }

            var coinInfo = await _coinService.GetCoinInfoAsync(alert.Coin.Name, 7);

            var alertDto = new PriceAlertDto
            {
                Id = alert.Id,
                UserId = alert.UserId,
                CoinId = alert.CoinId.ToString(),
                TargetPrice = alert.TargetPrice,
                AlertSetAt = alert.AlertSetAt,
                AlertTriggeredAt = alert.AlertTriggeredAt,
                IsNotified = alert.IsNotified,
                Coin = coinInfo
            };

            return alertDto;
        }


        private string ExtractUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");

            if (userIdClaim == null)
            {
                throw new Exception("User identifier not found in token.");
            }

            return userIdClaim.Value;
        }
    }
}
