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
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
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
                string.Equals(c.Name, coinName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Symbol, coinName, StringComparison.OrdinalIgnoreCase)) ?? throw new Exception($"Coin with name or symbol '{coinName}' not found.");
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
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(coinDto), options);

            return coinDto;
        }

        public async Task<IEnumerable<CoinDto>> GetFollowedCoins()
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

            List<decimal[]> prices;
            try
            {
                var marketChartResult = await _client.CoinsClient.GetMarketChartsByCoinId(coinId, "usd", days.ToString())
                    ?? throw new Exception("Market chart data is null.");

                prices = marketChartResult.Prices
                    .Select(innerArray => innerArray.Select(x => x ?? 0M).ToArray())
                    .ToList();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(prices), options);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("429"))
                {
                    var errorOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                    };
                    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(new { error = "429" }), errorOptions);
                    throw new Exception("Rate limit exceeded for CoinGecko API. Please try again later.");
                }
                throw;
            }
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
            string trendingCacheKey = "trending-coins";
            var cachedTrending = await _cache.GetStringAsync(trendingCacheKey);
            if (!string.IsNullOrEmpty(cachedTrending))
            {
                return JsonSerializer.Deserialize<IEnumerable<CoinDto>>(cachedTrending);
            }
            var allCoins = await GetAllCoinsAsync();
            var trendingCoins = allCoins.OrderBy(c => c.MarketCapRank).Take(count).ToList();
            var result = new List<CoinDto>();
            foreach (var coin in trendingCoins)
            {
                var coinData = await GetCoinInfoAsync(coin.Name, 7);
                result.Add(coinData);
                await Task.Delay(500);
            }
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            await _cache.SetStringAsync(trendingCacheKey, JsonSerializer.Serialize(result), options);

            return result;
        }

        public async Task<List<CoinDto>> GetMultipleCoinsDataAsync(IEnumerable<string> coinIds)
        {
            var uniqueIds = coinIds
                .Where(Id => !string.IsNullOrWhiteSpace(Id))
                .ToList();

            Console.WriteLine($"Coin IDs: {string.Join(", ", uniqueIds)}");

            if (uniqueIds.Count == 0)
                return [];

            var cacheKey = $"multiple-coins:{string.Join(",", uniqueIds)}";
            var cachedString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedString))
            {
                return JsonSerializer.Deserialize<List<CoinDto>>(cachedString);
            }
            var markets = await _client.CoinsClient.GetCoinMarkets(
                vsCurrency: "usd",
                ids: uniqueIds.ToArray(),
                order: "market_cap_desc",
                perPage: 250,
                page: 1,
                sparkline: false,
                priceChangePercentage: "", 
                category: "" 
            );
            var result = markets.Select(m => new CoinDto
            {
                Id = m.Id,
                Name = m.Name,
                Symbol = m.Symbol,
                CurrentPrice = m.CurrentPrice ?? 0,
                MarketCap = m.MarketCap ?? 0,
                TotalVolume = m.TotalVolume ?? 0,
                Thumb = m.Image.ToString(),
                Large = m.Image.ToString(), 
                MarketCapRank = (int)(m.MarketCapRank ?? 0)
            }).ToList();
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            await _cache.SetStringAsync(
                cacheKey, 
                JsonSerializer.Serialize(result), 
                options
            );
            return result;
        }

        public async Task<IEnumerable<CoinDto>> SearchCoinsAsync(string searchTerm)
        {
            var coins = await GetAllCoinsAsync();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return coins;
            }

            var lowerSearchTerm = searchTerm.ToLowerInvariant();

            var filteredCoins = coins.Where(c => 
                (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(lowerSearchTerm, StringComparison.InvariantCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.Symbol) && c.Symbol.Contains(lowerSearchTerm, StringComparison.InvariantCultureIgnoreCase))
            );

            return filteredCoins;
        }

        public async Task<List<decimal[]>> GetMarketOhlcByCoinIdAsync(string coinId, int days)
        {
            string cacheKey = $"market-ohlc:{coinId}:{days}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<decimal[]>>(cachedData);
            }

            List<decimal[]> ohlcData;
            try
            {
                var ohlcResult = await GetOhlcDataFromCoinGecko(coinId, days);
                ohlcData = [.. ohlcResult.Select(arr => arr.ToArray())];
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(ohlcData), options);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("429"))
                {
                    var errorOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                    };
                    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(new { error = "429" }), errorOptions);
                    throw new Exception("Rate limit exceeded for CoinGecko API. Please try again later.");
                }
                throw;
            }

            return ohlcData;
        }

        private async Task<List<decimal[]>> GetOhlcDataFromCoinGecko(string coinId, int days)
        {
                    
            var url = $"coins/{coinId}/ohlc?vs_currency=usd&days={days}";
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.coingecko.com/api/v3/")
            };

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CoinHawkApp/1.0");
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<List<decimal?>>>(jsonString) ?? throw new Exception("OHLC data is null");

            return data.Select(arr => arr.Select(x => x ?? 0M).ToArray()).ToList();
        }


    }
}
