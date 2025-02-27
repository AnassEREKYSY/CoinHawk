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
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class PriceAlertService(ApplicationContext _context,ICoinService _coinService,INewsService _newsService) : IPriceAlertService
    {

        public async Task<PriceAlert> CreatePriceAlertAsync(string userToken, string coinName, decimal targetPrice)
        {
            var userEmail = ExtractUserIdFromToken(userToken);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail) ?? throw new Exception("User not found in database.");
            var coin = await _coinService.GetCoinDataByNameAsync(coinName);
            
            var priceAlert = new PriceAlert
            {
                UserId = user.Id, 
                CoinName = coin.Name,
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
        TargetPrice = alert.TargetPrice,
        AlertSetAt = alert.AlertSetAt,
        AlertTriggeredAt = alert.AlertTriggeredAt,
        IsNotified = alert.IsNotified,
    }).ToList();

    return result;
}


        public async Task<PriceAlertDto> GetAlertForUserAsync(int alertId, string token)
        {
            var userId = ExtractUserIdFromToken(token);

            var alert = await _context.PriceAlerts.FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

            if (alert == null)
            {
                throw new Exception("Alert not found or you are not authorized to access this alert.");
            }

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
