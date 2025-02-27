using Infrastructure.Dtos;
using Infrastructure.IServices;
using CoinGecko.Clients;

namespace Infrastructure.Services
{
    public class CoinService(CoinGeckoClient _client) : ICoinService
    {
        public async Task<IEnumerable<CoinDto>> GetAllCoinsAsync()
        {
            var coinListResult = await _client.CoinsClient.GetCoinList() ?? throw new Exception("Failed to retrieve the coin list.");
            return coinListResult.Select(c => new CoinDto
            {
                Id = c.Id,
                Name = c.Name,
                Symbol = c.Symbol,
            });
        }

        public async Task<IEnumerable<CoinDto>> GetFollowedCoins(string userToken)
        {
            var coinListResult = await _client.CoinsClient.GetCoinList() ?? throw new Exception("Failed to retrieve the coin list.");
            return coinListResult.Select(c => new CoinDto
            {
                Id = c.Id,
                Name = c.Name,
                Symbol = c.Symbol,
            });
        }
        
        public async Task<CoinDto> GetCoinDataByNameAsync(string coinName)
        {
            var coins = await GetAllCoinsAsync();
            var coin = coins.FirstOrDefault(c => string.Equals(c.Name, coinName, StringComparison.OrdinalIgnoreCase)) ?? throw new Exception($"Coin with name '{coinName}' not found.");
            var coinData = await _client.CoinsClient.GetAllCoinDataWithId(coin.Id) ?? throw new Exception($"Failed to retrieve data for coin ID: {coin.Id}");
            return new CoinDto
            {
                Id = coinData.Id,
                Name = coinData.Name,
                Symbol = coinData.Symbol,
                CurrentPrice = coinData.MarketData?.CurrentPrice.GetValueOrDefault("usd", 0) ?? 0,
                MarketCap = coinData.MarketData?.MarketCap.GetValueOrDefault("usd", 0) ?? 0,
                TotalVolume = coinData.MarketData?.TotalVolume.GetValueOrDefault("usd", 0) ?? 0,
                Thumb = coinData.Image?.Thumb?.ToString() ?? string.Empty, 
                Large = coinData.Image?.Large?.ToString() ?? string.Empty
            };
        }

        public async Task<List<decimal[]>> GetMarketChartByCoinIdAsync(string coinId, int days)
        {
            var marketChartResult = await _client.CoinsClient.GetMarketChartsByCoinId(coinId, "usd", days.ToString()) ?? throw new Exception("Failed to retrieve market chart data.");
            var prices = marketChartResult.Prices
                .Select(innerArray => innerArray.Select(x => x ?? 0M).ToArray())
                .ToList();
                
            return prices;
        }

        public async Task<CoinDto> GetCoinInfoAsync(string coinName, int days)
        {
            var coinData = await GetCoinDataByNameAsync(coinName);
            coinData.MarketChart = await GetMarketChartByCoinIdAsync(coinData.Id, days);
            return coinData;
        }

        public async Task<IEnumerable<CoinDto>> GetTrendingCoinsAsync(int count = 10)
        {
            var allCoins = await GetAllCoinsAsync();
            var trendingCoins = allCoins.OrderBy(c => c.MarketCapRank).Take(count).ToList();

            var result = new List<CoinDto>();
            foreach (var coin in trendingCoins)
            {
                var coinData = await GetCoinInfoAsync(coin.Name, 7);
                result.Add(coinData);
            }

            return result;
        }
    }
}
