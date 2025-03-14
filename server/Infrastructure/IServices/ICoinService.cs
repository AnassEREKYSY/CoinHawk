using Infrastructure.Dtos;

namespace Infrastructure.IServices
{
    public interface ICoinService
    {
        Task<IEnumerable<CoinDto>> GetAllCoinsAsync();
        Task<CoinDto> GetCoinDataByNameAsync(string coinName);
        Task<List<decimal[]>> GetMarketChartByCoinIdAsync(string coinId, int days);
        Task<CoinDto> GetCoinInfoAsync(string coinName, int days);
        Task<IEnumerable<CoinDto>> GetTrendingCoinsAsync(int count = 10);
        Task<IEnumerable<CoinDto>> GetFollowedCoins();
        Task<List<CoinDto>> GetMultipleCoinsDataAsync(IEnumerable<string> coinIds);
        Task<IEnumerable<CoinDto>> SearchCoinsAsync(string searchTerm);
        Task<List<decimal[]>> GetMarketOhlcByCoinIdAsync(string coinId, int days);
    }
}
