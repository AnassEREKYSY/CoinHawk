using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Dtos;

namespace API.Controllers
{
    [ApiController]
    [Route("api/price-alerts")]
    public class PriceAlertsController(IPriceAlertService _priceAlertService, IJwtTokenDecoderService _jwtTokenDecoderService) : ControllerBase
    {

        [HttpPost("create-alert")]
        public async Task<IActionResult> CreatePriceAlert([FromBody] CreatePriceAlertDto createAlertDto)
        {
            var token = _jwtTokenDecoderService.GetTokenFromHeaders(Request);
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
            var token = _jwtTokenDecoderService.GetTokenFromHeaders(Request);
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
            var token = _jwtTokenDecoderService.GetTokenFromHeaders(Request);
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
            var token = _jwtTokenDecoderService.GetTokenFromHeaders(Request);
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
            var token = _jwtTokenDecoderService.GetTokenFromHeaders(Request);
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
            var token = _jwtTokenDecoderService.GetTokenFromHeaders(Request);
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

        [HttpDelete("delete-alerts-by-coin/{coinId}")]
        public async Task<IActionResult> DeleteAlertsByCoin(string coinId)
        {
            var token = _jwtTokenDecoderService.GetTokenFromHeaders(Request);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }
            
            try
            {
                await _priceAlertService.DeletePriceAlertsByCoinAndTargetPriceAsync(coinId, token);
                return Ok($"Alerts for coin {coinId} is successfully deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
