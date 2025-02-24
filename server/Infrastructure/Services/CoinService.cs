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
        public async Task<List<CoinDto>> SearchAndDisplayCoinsAsync(string coinName)
        {
            var searchResult = await _client.CoinsClient.GetAllCoinsData();
            var filteredCoins = searchResult
                .Where(coin => coin.Name.Contains(coinName, StringComparison.OrdinalIgnoreCase))
                .Select(coin => new CoinDto
                {
                    Id = coin.Id,
                    Name = coin.Name,
                    Symbol = coin.Symbol,
                })
                .ToList();

            return filteredCoins;
        }
    }
}
