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

namespace Infrastructure.Services
{
    public class CoinService(CoinGeckoClient _client) : ICoinService
    {

        public async Task<IEnumerable<CoinDto>> GetAllCoinsAsync()
        {
            var coinListResult = await _client.CoinsClient.GetCoinListAsync();
            if (coinListResult.Success)
            {
                return coinListResult.Data.Select(c => new CoinDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ApiSymbol = c.ApiSymbol,
                    Symbol = c.Symbol,
                    MarketCapRank = c.MarketCapRank ?? 0,
                    Thumb = c.Thumb,
                    Large = c.Large
                });
            }
            throw new Exception("Failed to retrieve the coin list.");
        }

        public async Task<CoinDto> GetCoinDataByNameAsync(string coinName)
        {
            var coins = await this.GetAllCoinsAsync();
            var coin = coins.FirstOrDefault(c => string.Equals(c.Name, coinName, StringComparison.OrdinalIgnoreCase));
            if (coin != null)
            {
                var coinDataResult = await _client.CoinsClient.GetAllCoinDataWithIdAsync(coin.Id);
                if (coinDataResult.Success)
                {
                    var data = coinDataResult.Data;
                    coin.CurrentPrice = data.MarketData.CurrentPrice.GetValueOrDefault("usd", 0);
                    coin.MarketCap = data.MarketData.MarketCap.GetValueOrDefault("usd", 0);
                    coin.TotalVolume = data.MarketData.TotalVolume.GetValueOrDefault("usd", 0);
                    return coin;
                }
                throw new Exception($"Failed to retrieve data for coin ID: {coin.Id}");
            }
            throw new Exception($"Coin with name '{coinName}' not found.");
        }

        public async Task<List<decimal[]>> GetMarketChartByCoinIdAsync(string coinId, int days)
        {
            var marketChartResult = await _client.CoinsClient.GetMarketChartsByCoinIdAsync(coinId, "usd", days);
            if (marketChartResult.Success)
            {
                return marketChartResult.Data.Prices;
            }
            throw new Exception("Failed to retrieve market chart data.");
        }

         public async Task<CoinDto> GetCoinInfoAsync(string coinName, int days)
        {
            var coinData = await this.GetCoinDataByNameAsync(coinName);
            coinData.MarketChart = await this.GetMarketChartByCoinIdAsync(coinData.Id, days);
            return coinData;
        }
    }
}
