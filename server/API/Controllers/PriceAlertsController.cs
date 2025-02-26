using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Infrastructure.Dtos
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("api/price-alerts")]
    [Authorize]
    public class PriceAlertsController(IPriceAlertService _priceAlertService) : ControllerBase
    {

        [HttpPost("create-alert")]
        public async Task<IActionResult> CreatePriceAlert([FromBody] CreatePriceAlertDto createAlertDto)
        {
            var token = ExtractJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }

            try
            {
                var alert = await _priceAlertService.CreatePriceAlertAsync(token, createAlertDto.CoinId, createAlertDto.TargetPrice);
                return Ok(alert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("user-alerts")]
        public async Task<IActionResult> GetAllUserAlerts()
        {
            var token = ExtractJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }

            try
            {
                var alerts = await _priceAlertService.GetAllAlertsForUserAsync(token);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user-one-alert/{alertId}")]
        public async Task<IActionResult> GetUserAlert(int alertId)
        {
            var token = ExtractJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }

            try
            {
                var alert = await _priceAlertService.GetAlertForUserAsync(alertId, token);
                return Ok(alert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("coin-news")]
        public async Task<IActionResult> GetNewsForFollowedCoins()
        {
            var token = ExtractJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }

            try
            {
                var newsArticles = await _priceAlertService.GetNewsForFollowedCoinsAsync(token);
                return Ok(newsArticles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string ExtractJwtToken()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return null;
            }
            var token = authHeader.ToString();
            if (token.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            return token;
        }
    }
}
