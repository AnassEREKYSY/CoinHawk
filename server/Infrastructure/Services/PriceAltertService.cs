using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Infrastructure.Dtos;
using Core.Entities;
using Infrastructure.IServices;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class PriceAlertService(ApplicationContext _context,ICoinService _coinService,INewsService _newsService) : IPriceAlertService
    {

        public async Task<PriceAlert> CreatePriceAlertAsync(string userToken, string coinName, decimal targetPrice)
        {
            var userEmail = ExtractUserIdFromToken(userToken);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail)
                ?? throw new Exception("User not found in database.");
            var coin = await _coinService.GetCoinDataByNameAsync(coinName);

            var priceAlert = new PriceAlert
            {
                UserId = user.Id,
                CoinName = coin.Name,
                coinId = coin.Id,
                TargetPrice = targetPrice,
                AlertSetAt = DateTime.UtcNow,
                IsNotified = false
            };

            _context.PriceAlerts.Add(priceAlert);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }

            return priceAlert;
        }

        public async Task<IEnumerable<PriceAlertDto>> GetAllAlertsForUserAsync(string token)
        {
            var userEmail = ExtractUserIdFromToken(token);
            Console.WriteLine($"Extracted User Email from Token: {userEmail}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            
            if (user == null)
            {
                Console.WriteLine("User not found in database.");
                throw new Exception("User not found in database.");
            }

            Console.WriteLine($"User found: {user.Id} (Email: {user.Email})");

            var alerts = await _context.PriceAlerts
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            Console.WriteLine($"Total alerts found: {alerts.Count}");

            var result = alerts.Select(alert => new PriceAlertDto
            {
                Id = alert.Id,
                UserId = alert.UserId,
                CoinName = alert.CoinName,
                CoinId = alert.coinId,
                TargetPrice = alert.TargetPrice,
                AlertSetAt = alert.AlertSetAt,
                AlertTriggeredAt = alert.AlertTriggeredAt,
                IsNotified = alert.IsNotified
            }).ToList();

            return result;
        }

        public async Task<PriceAlertDto> GetAlertForUserAsync(int alertId, string token)
        {
            var userId = ExtractUserIdFromToken(token);

            var alert = await _context.PriceAlerts.FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId)
                ?? throw new Exception("Alert not found or you are not authorized to access this alert.");
            var coinInfo = await _coinService.GetCoinInfoAsync(alert.CoinName, 7);

            var alertDto = new PriceAlertDto
            {
                Id = alert.Id,
                UserId = alert.UserId,
                CoinName = alert.CoinName,
                TargetPrice = alert.TargetPrice,
                AlertSetAt = alert.AlertSetAt,
                AlertTriggeredAt = alert.AlertTriggeredAt,
                IsNotified = alert.IsNotified,
                Coin = coinInfo
            };

            return alertDto;
        }

        public async Task<IEnumerable<NewsArticleDto>> GetNewsForFollowedCoinsAsync(string token)
        {
            var alerts = await GetAllAlertsForUserAsync(token);

            var followedCoins = alerts
                .Select(alert => alert.Coin.Name)
                .Distinct()
                .ToList();

            var newsArticles = new List<NewsArticleDto>();

            foreach (var coin in followedCoins)
            {
                var articles = await _newsService.GetNewsForCoinAsync(coin);
                newsArticles.Add(articles);
            }

            return newsArticles;
        }

        public async Task<IEnumerable<CoinDto>> ExtractFollowedCoinsFromAlerts(string userToken)
        {
            var alerts = await GetAllAlertsForUserAsync(userToken);
            var coinIds = alerts.Select(a => a.CoinId).ToList();
            var coinsData = await _coinService.GetMultipleCoinsDataAsync(coinIds);
            return coinsData;
        }

        public async Task DeletePriceAlertAsync(int alertId, string token)
        {
            var userEmail = ExtractUserIdFromToken(token);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            
            if (user == null)
            {
                Console.WriteLine("User not found in database.");
                throw new Exception("User not found in database.");
            }

            var alert = await _context.PriceAlerts.FirstOrDefaultAsync(a => a.Id == alertId) ?? throw new Exception("Alert not found.");

            if (alert.UserId != user.Id)
            {
                throw new Exception("You are not authorized to delete this alert.");
            }

            _context.PriceAlerts.Remove(alert);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task DeletePriceAlertsByCoinAndTargetPriceAsync(string coinId, decimal targetPrice, string token)
        {
            var userEmail = ExtractUserIdFromToken(token);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            
            if (user == null)
            {
                Console.WriteLine("User not found in database.");
                throw new Exception("User not found in database.");
            }

            var alerts = await _context.PriceAlerts
                .Where(a => a.UserId == user.Id && a.coinId == coinId && a.TargetPrice == targetPrice)
                .ToListAsync();

            if (alerts.Count == 0)
            {
                throw new Exception("No alerts found with the specified criteria.");
            }

            _context.PriceAlerts.RemoveRange(alerts);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting alerts: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        private string ExtractUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub") ?? throw new Exception("User identifier not found in token.");
            return userIdClaim.Value;
        }

    }
}
