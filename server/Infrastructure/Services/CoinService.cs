using Infrastructure.Dtos;
using Infrastructure.IServices;
using CoinGecko.Clients;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class CoinService(CoinGeckoClient _client, IDistributedCache _cache) : ICoinService
    {
        public async Task<IEnumerable<CoinDto>> GetAllCoinsAsync()
        {
            string cacheKey = "all-coins";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<CoinDto>>(cachedData);
            }

            var coinListResult = await _client.CoinsClient.GetCoinList()
                ?? throw new Exception("Failed to retrieve the coin list.");

            var coinDtos = coinListResult.Select(c => new CoinDto
            {
                Id = c.Id,
                Name = c.Name,
                Symbol = c.Symbol
            });

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(coinDtos), options);

            return coinDtos;
        }

        public async Task<CoinDto> GetCoinDataByNameAsync(string coinName)
        {
            string cacheKey = $"coin-data:{coinName.ToLower()}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<CoinDto>(cachedData);
            }
            var coins = await GetAllCoinsAsync();
            var coin = coins.FirstOrDefault(c =>
                string.Equals(c.Name, coinName, StringComparison.OrdinalIgnoreCase))
                ?? throw new Exception($"Coin with name '{coinName}' not found.");

            var coinData = await _client.CoinsClient.GetAllCoinDataWithId(coin.Id)
                ?? throw new Exception($"Failed to retrieve data for coin ID: {coin.Id}");

            var coinDto = new CoinDto
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
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(coinDto), options);

            return coinDto;
        }

        public async Task<IEnumerable<CoinDto>> GetFollowedCoins(string userToken)
        {

            var coinListResult = await GetAllCoinsAsync();
            return coinListResult;
        }

        public async Task<List<decimal[]>> GetMarketChartByCoinIdAsync(string coinId, int days)
        {
            string cacheKey = $"market-chart:{coinId}:{days}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<decimal[]>>(cachedData);
            }
            var marketChartResult = await _client.CoinsClient.GetMarketChartsByCoinId(coinId, "usd", days.ToString())
                ?? throw new Exception("Failed to retrieve market chart data.");

            var prices = marketChartResult.Prices
                .Select(innerArray => innerArray.Select(x => x ?? 0M).ToArray())
                .ToList();

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(prices), options);
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
            var trendingCoins = allCoins
                .OrderBy(c => c.MarketCapRank)
                .ToList();

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
