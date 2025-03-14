using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Infrastructure.Dtos;
using Core.Entities;
using Infrastructure.IServices;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class PriceAlertService(ApplicationContext _context,ICoinService _coinService,INewsService _newsService, IJwtTokenDecoderService _jwtTokenDecoderService) : IPriceAlertService
    {

        public async Task<PriceAlert> CreatePriceAlertAsync(string userToken, string coinName, decimal targetPrice)
        {
            var payload = _jwtTokenDecoderService.GetTokenPayload(userToken);
            var userEmail = payload.TryGetValue("email", out var emailObj) ? emailObj?.ToString() : null;
            var coin = await _coinService.GetCoinDataByNameAsync(coinName);

            var existingAlert = await _context.PriceAlerts.FirstOrDefaultAsync(a =>
                a.UserEmail == userEmail && a.CoinId == coin.Id);
                
            if (existingAlert != null)
            {
                existingAlert.TargetPrice = targetPrice;
                existingAlert.AlertSetAt = DateTime.UtcNow;
                
                _context.PriceAlerts.Update(existingAlert);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating alert: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                    throw;
                }
                return existingAlert;
            }
            else
            {
                var priceAlert = new PriceAlert
                {
                    UserEmail = userEmail,
                    CoinName = coin.Name,
                    CoinId = coin.Id,
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
                    Console.WriteLine($"Error creating alert: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                    throw;
                }

                return priceAlert;
            }
        }

        public async Task<IEnumerable<PriceAlertDto>> GetAllAlertsForUserAsync(string token)
        {
            var payload = _jwtTokenDecoderService.GetTokenPayload(token);
            var userEmail = payload.TryGetValue("email", out var emailObj) ? emailObj?.ToString() : null;
            var alerts = await _context.PriceAlerts
                .Where(a => a.UserEmail == userEmail)
                .ToListAsync();
            var result = alerts.Select(alert => new PriceAlertDto
            {
                Id = alert.Id,
                UserEmail = userEmail,
                CoinName = alert.CoinName,
                CoinId = alert.CoinId,
                TargetPrice = alert.TargetPrice,
                AlertSetAt = alert.AlertSetAt,
                AlertTriggeredAt = alert.AlertTriggeredAt,
                IsNotified = alert.IsNotified
            }).ToList();

            return result;
        }

        public async Task<PriceAlertDto> GetAlertForUserAsync(int alertId, string token)
        {
            var payload = _jwtTokenDecoderService.GetTokenPayload(token);
            var userEmail = payload.TryGetValue("email", out var emailObj) ? emailObj?.ToString() : null;
            var alert = await _context.PriceAlerts.FirstOrDefaultAsync(a => a.Id == alertId && a.UserEmail == userEmail)
                ?? throw new Exception("Alert not found or you are not authorized to access this alert.");
            var coinInfo = await _coinService.GetCoinInfoAsync(alert.CoinName, 7);

            var alertDto = new PriceAlertDto
            {
                Id = alert.Id,
                UserEmail = userEmail,
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
            foreach (var coin in coinsData)
            {
                var alertForCoin = alerts.FirstOrDefault(a => a.CoinId == coin.Id);
                if (alertForCoin != null)
                {
                    coin.TargetPrice = alertForCoin.TargetPrice;
                }
            }
            return coinsData;
        }

        public async Task DeletePriceAlertAsync(int alertId, string token)
        {
            var payload = _jwtTokenDecoderService.GetTokenPayload(token);
            var userEmail = payload.TryGetValue("email", out var emailObj) ? emailObj?.ToString() : null;
            var alert = await _context.PriceAlerts.FirstOrDefaultAsync(a => a.Id == alertId) ?? throw new Exception("Alert not found.");

            if (alert.UserEmail != userEmail)
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

        public async Task DeletePriceAlertsByCoinAndTargetPriceAsync(string coinId, string token)
        {
            var payload = _jwtTokenDecoderService.GetTokenPayload(token);
            var userEmail = payload.TryGetValue("email", out var emailObj) ? emailObj?.ToString() : null;
            var alerts = await _context.PriceAlerts
                .Where(a => a.UserEmail == userEmail && a.CoinId == coinId)
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
    }
}
