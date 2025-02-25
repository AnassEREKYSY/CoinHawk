using Core.Entities;

namespace Infrastructure.IServices
{
    public interface IPriceAltertService
    {
        Task<PriceAlert> CreatePriceAlertAsync(string userToken, int coinId, decimal targetPrice);
    }
}
