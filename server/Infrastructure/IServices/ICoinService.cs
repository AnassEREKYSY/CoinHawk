using Infrastructure.Dtos;

namespace Infrastructure.IServices
{
    public interface ICoinService
    {
        Task<IEnumerable<CoinDto>> GetAllCoinsAsync();
        Task<CoinDto> GetCoinDataByNameAsync(string coinName);
        Task<List<decimal[]>> GetMarketChartByCoinIdAsync(string coinId, int days);
        Task<CoinDto> GetCoinInfoAsync(string coinName, int days);
        Task<IEnumerable<CoinDto>> GetTrendingCoinsAsync(int count = 10)
    }
}
