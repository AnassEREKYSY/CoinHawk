using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Dtos;

namespace API.Controllers
{
    [ApiController]
    [Route("api/price-alerts")]
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
                var alert = await _priceAlertService.CreatePriceAlertAsync(token, createAlertDto.CoinName, createAlertDto.TargetPrice);
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

        [HttpGet("get-followed-coins")]
        public async Task<IActionResult> GetFollowedCoins()
        {
            var token = ExtractJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }

            try
            {
                var followedCoins = await _priceAlertService.ExtractFollowedCoinsFromAlerts(token);
                return Ok(followedCoins);
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


        [HttpDelete("delete-alert/{alertId}")]
        public async Task<IActionResult> DeletePriceAlert(int alertId)
        {
            var token = ExtractJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }
            
            try
            {
                await _priceAlertService.DeletePriceAlertAsync(alertId, token);
                return Ok($"Alert with Id {alertId} was successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("delete-alerts-by-coin")]
        public async Task<IActionResult> DeleteAlertsByCoin([FromQuery] string coinId, [FromQuery] decimal targetPrice)
        {
            var token = ExtractJwtToken();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }
            
            try
            {
                await _priceAlertService.DeletePriceAlertsByCoinAndTargetPriceAsync(coinId, targetPrice, token);
                return Ok($"Alerts for coin {coinId} with target price {targetPrice} were successfully deleted.");
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
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return token.Substring("Bearer ".Length).Trim();
            }

            return token;
        }

    }
}
