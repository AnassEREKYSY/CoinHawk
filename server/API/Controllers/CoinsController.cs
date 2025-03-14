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
                
                if (coinData == null)
                {
                    return NotFound($"Coin data for '{coinName}' not found.");
                }

                var marketChart = await _coinService.GetMarketChartByCoinIdAsync(coinData.Id, days);
                if (marketChart == null || !marketChart.Any())
                {
                    return NotFound($"Market chart data for '{coinName}' not found.");
                }

                return Ok(marketChart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

        [HttpGet("search-coin")]
        public async Task<IActionResult> SearchCoins([FromQuery] string query)
        {
            try
            {
                var result = await _coinService.SearchCoinsAsync(query);
                if (result == null || !result.Any())
                {
                    return NotFound("No matching coins found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       
        [HttpGet("get-coin-ohlc/{coinName}")]
        public async Task<IActionResult> GetCoinOhlcData(string coinName, [FromQuery] int days)
        {
            try
            {
                var coinData = await _coinService.GetCoinDataByNameAsync(coinName);
                if (coinData == null)
                {
                    return NotFound($"Coin data for '{coinName}' not found.");
                }

                var ohlcData = await _coinService.GetMarketOhlcByCoinIdAsync(coinData.Id, days);
                if (ohlcData == null || !ohlcData.Any())
                {
                    return NotFound($"OHLC data for '{coinName}' not found.");
                }
                return Ok(ohlcData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
