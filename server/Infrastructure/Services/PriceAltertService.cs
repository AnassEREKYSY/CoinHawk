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
    public class PriceAltertService(ApplicationDbContext _context) : IPriceAlertService
    {

        public async Task<PriceAlert> CreatePriceAlertAsync(string userToken, int coinId, decimal targetPrice)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(userToken);
            var userIdClaim = jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");

            if (userIdClaim == null)
            {
                throw new Exception("Invalid user token: User identifier not found.");
            }

            var userId = userIdClaim.Value;
            var priceAlert = new PriceAlert
            {
                UserId = userId,
                CoinId = coinId,
                TargetPrice = targetPrice,
                AlertSetAt = DateTime.UtcNow,
                IsNotified = false
            };

            _context.PriceAlerts.Add(priceAlert);
            await _context.SaveChangesAsync();

            return priceAlert;
        }
    }
}
