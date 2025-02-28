using Core.Entities;
using Infrastructure.Dtos;

namespace Infrastructure.IServices
{
    public interface IPriceAlertService
    {
        Task<PriceAlert> CreatePriceAlertAsync(string userToken, string coinName, decimal targetPrice);
        Task<IEnumerable<PriceAlertDto>> GetAllAlertsForUserAsync(string token);
        Task<PriceAlertDto> GetAlertForUserAsync(int alertId, string token);
        Task<IEnumerable<NewsArticleDto>> GetNewsForFollowedCoinsAsync(string token);
        Task<IEnumerable<CoinDto>> ExtractFollowedCoinsFromAlerts(string userToken);
    }
}
