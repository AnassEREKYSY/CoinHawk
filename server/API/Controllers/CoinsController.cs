using Infrastructure.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/coins")]
    public class CoinsController(ICoinService _coinService) : ControllerBase
    {

        [HttpGet("get-all-coins")]
        public async Task<IActionResult> GetAllCoins()
        {
            var result = await _coinService.GetAllCoinsAsync();
            if (result == null || !result.Any())
                return NotFound("No coins available.");

            return Ok(result);
        }

        [HttpGet("get-coin-data/{coinName}")]
        public async Task<IActionResult> GetCoinData(string coinName)
        {
            try
            {
                var result = await _coinService.GetCoinDataByNameAsync(coinName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("get-coin-market-chart/{coinName}/market-chart")]
        public async Task<IActionResult> GetCoinMarketChart(string coinName, [FromQuery] int days)
        {
            try
            {
                var coinData = await _coinService.GetCoinDataByNameAsync(coinName);
                var marketChart = await _coinService.GetMarketChartByCoinIdAsync(coinData.Id, days);
                return Ok(marketChart);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("get-coin-details-info/{coinName}")]
        public async Task<IActionResult> GetCoinInfo(string coinName, [FromQuery] int days)
        {
            try
            {
                var result = await _coinService.GetCoinInfoAsync(coinName, days);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("get-trending-coins")]
        public async Task<IActionResult> GetTrendingCoins()
        {
            var trendingCoins = await _coinService.GetTrendingCoinsAsync(10);
            return Ok(trendingCoins);
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
                var followedCoins = await _coinService.GetFollowedCoins(token);
                return Ok(followedCoins);
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
